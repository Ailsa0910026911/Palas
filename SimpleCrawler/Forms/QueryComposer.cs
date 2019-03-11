using Crawler.Core.Utility;
using HooLab.Log;
using Palas.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Host
{
    interface IWeiboQueryComposer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="query"></param>
        void Initialize(SinaSearch.WeiboSearchQuery query);

        /// <summary>
        /// 返回下一个搜索任务对象
        /// </summary>
        /// <returns>如果是null则表示已经完毕</returns>
        SinaSearch.WeiboSearchQuery Next();

        /// <summary>
        /// 检查是否还有搜索任务
        /// </summary>
        /// <returns></returns>
        Boolean Empty { get; }

        /// <summary>
        /// 把需要的信息反馈回来，据此判断是否还有新的request
        /// </summary>
        /// <param name="status"></param>
        void ReportStatus(ref Item[] result, int count);
    }

    internal class QueryComposerFactory
    {
        internal static IWeiboQueryComposer GetQueryComposer(SinaSearch.WeiboSearchQuery query)
        {
            IWeiboQueryComposer composer = null;
            if (query.SearchAll == true)
            {
                composer = new TimePeriodQueryComposer();
            }
            else
            {
                composer = new SimpleQueryComposer();
            }
            composer.Initialize(query);

            return composer;
        }
    }

    internal sealed class SimpleQueryComposer : IWeiboQueryComposer
    {
        private SinaSearch.WeiboSearchQuery _query;

        public void Initialize(SinaSearch.WeiboSearchQuery query)
        {
            _query = query;
        }

        public SinaSearch.WeiboSearchQuery Next()
        {
            var once = _query;
            _query = null;
            return once;
        }


        public Boolean Empty
        {
            get { return _query == null; }
        }


        public void ReportStatus(ref Item[] result, int count)
        {
            // 不需要管这个
        }
    }

    /// <summary>
    /// 基于区域分组的生成算法
    /// </summary>
    internal sealed class RegionQueryComposer : IWeiboQueryComposer
    {
        private DateTime _start;
        private DateTime _end;
        private String _keyword;
        private String _addtionQuery;
        private Boolean _isOrigin;
        private Boolean _isVip;

        private Int32 _currentRegion = 0;
        private Int32 _currentSubregion = 0;

        private Boolean _finished = false;

        public readonly Int32[] Regions = new Int32[] { 34, 11, 50, 35, 62, 44, 45, 52, 46, 13, 23, 41, 42, 43, 15, 32, 36, 22, 21, 64, 63, 14, 37, 31, 51, 12, 54, 65, 53, 33, 61, 71, 81, 82, 400, 100 };

        public readonly Int32[][] Subregions = new Int32[][]
        {
            new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
            new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 28, 29 },
            new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 40, 41, 42, 43, 81, 82, 83, 84 },
            new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
            new Int32[] {},
        };

        public void Initialize(SinaSearch.WeiboSearchQuery query)
        {
            _start = (DateTime)query.StartDate;
            _end = (DateTime)query.EndDate;
            _keyword = query.Keyword;
            // nodup是为了返回所有结果，不要忽略
            _addtionQuery = String.Format("{0}&{1}", query.AddtionQuery, "nodup=1");
            _isOrigin = query.isOrigin == true;
            _isVip = query.isVip == true;
        }

        public SinaSearch.WeiboSearchQuery Next()
        {
            if (_finished)
            {
                return null;
            }

            String regionQuery = String.Format(
                "{0}&region=custom:{1}:{2}", _addtionQuery, Regions[_currentRegion], 1000);

            var query = new SinaSearch.WeiboSearchQuery()
            {
                AddtionQuery = regionQuery,
                Keyword = _keyword,
                isOrigin = _isOrigin,
                isVip = _isVip,
                StartPage = 1,
                EndPage = 50,
                Option = SearchWeiboOption.RealTime,
                SearchAll = true,
                StartDate = _start,
                EndDate = _end,
            };

            return query;
        }

        public bool Empty
        {
            get { return _finished; }
        }

        public void ReportStatus(ref Item[] result, int count)
        {
            if (count < 0)
            {
                // 应该是被弹验证码了，等待一段时间看看
                Thread.Sleep(TimeSpan.FromMinutes(10));
                return;
            }

            if (count > TimePeriodQueryComposer.MaximumItemPerRequest)
            {
                System.Diagnostics.Debug.WriteLine("Region > 1000");
                Logger.Warn(String.Format("{0},{1},{2} exceeds 1000", _keyword, Regions[_currentRegion], _start));
            }

            // Test
            foreach (var item in result)
            {
                item.Lat = _currentRegion;
            }

            if (_currentRegion == Regions.Length - 1)
            {
                _finished = true;
                return;
            }

            _currentRegion++;
        }
    }

    /// <summary>
    /// 按照月-日-小时的跨度来搜索，一个跨度如果超过1000条微薄
    /// 则细分到下一个跨度再进行搜索，如果到小时级别还是大于1000条则使用RegionQueryComposer
    /// </summary>
    internal sealed class TimePeriodQueryComposer : IWeiboQueryComposer
    {
        private DateTime _start;
        private DateTime _end;
        private String _keyword;
        private String _addtionQuery;
        private Boolean _isOrigin;
        private Boolean _isVip;

        private DateTime _currentEnd;
        private DateTime _currentStart;
        private Boolean _finished = false;
        private Boolean _fallToRegion = false;
        private readonly TimeSpan _advance = TimeSpan.FromDays(30);
        private readonly TimeSpan _hour = TimeSpan.FromHours(1);

        private HashSet<String> _lastQueryItems = new HashSet<String>();
        private IWeiboQueryComposer _regionComposer = null;

        public const int MaximumItemPerRequest = 1000;
        public const int ConservativeItemPerRequest = 150;

        public void Initialize(SinaSearch.WeiboSearchQuery query)
        {
            _start = (DateTime)query.StartDate;
            _end = (DateTime)query.EndDate;
            _keyword = query.Keyword;
            // nodup是为了返回所有结果，不要忽略
            _addtionQuery = String.Format("{0}&{1}", query.AddtionQuery, "nodup=1");
            _isOrigin = query.isOrigin == true;
            _isVip = query.isVip == true;

            _currentEnd = _end;
        }

        public SinaSearch.WeiboSearchQuery Next()
        {
            if (_finished)
            {
                return null;
            }

            if (_fallToRegion && _regionComposer != null)
            {
                return _regionComposer.Next();
            }

            DateTime estimatedStart = _currentEnd - _advance;
            _currentStart = estimatedStart < _start ? _start : estimatedStart;

            var query = new SinaSearch.WeiboSearchQuery()
            {
                AddtionQuery = _addtionQuery,
                Keyword = _keyword,
                isOrigin = _isOrigin,
                isVip = _isVip,
                StartPage = 1,
                EndPage = 50,
                Option = SearchWeiboOption.RealTime,
                SearchAll = true,
                StartDate = _currentStart,
                EndDate = _currentEnd,
            };

            return query;
        }

        public Boolean Empty
        {
            get { return _finished; }
        }

        public void ReportStatus(ref Item[] result, int count)
        {
            Logger.Warn(String.Format("ReportStatus count {0}, returned {1}", count, result.Length));

            if (count < 0)
            {
                // 应该是被弹验证码了，等待一段时间看看
                //Thread.Sleep(TimeSpan.FromMinutes(10));
                return;
            }

            if (_fallToRegion && _regionComposer != null)
            {
                _regionComposer.ReportStatus(ref result, count);
                if (_regionComposer.Empty)
                {
                    _fallToRegion = false;
                    _regionComposer = null;
                    Logger.Warn(String.Format("Region composer finished, next time end {0}", _currentEnd));
                }
                return;
            }

            // 如果显示值大于MaximumItemPerRequest，但是返回值小于ConservativeItemPerRequest
            // 则认为这一次请求不完整，跳过重新发送请求
            if (count > MaximumItemPerRequest && result.Length < ConservativeItemPerRequest)
            {
                return;
            }

            // 有的时候count会返回0，但是实际上有item返回
            if (count <= MaximumItemPerRequest)
            {
                if (count < 0)
                {
                    Logger.Warn("Fail to retrieve count element");
                    _finished = true;
                    return;
                }
                Logger.Warn(String.Format("ReportStatus count {0} is less than {1}", count, MaximumItemPerRequest));
                var latest = result.LastOrDefault();
                _currentEnd = latest == null ? _currentEnd : latest.PubDate.Value;
            }
            else
            {
                Item last = result.LastOrDefault();
                DateTime lastDate = last.PubDate.Value;
                Logger.Warn(String.Format("First {0}, Last {1}", result[0].PubDate, last.PubDate));

                // 一个小时内超过1000条，这一个小时全部使用基于区域分组的算法
                if (_currentEnd - lastDate < _hour && _currentEnd.Hour == lastDate.Hour)
                {
                    _fallToRegion = true;
                    _regionComposer = new RegionQueryComposer();
                    DateTime hour = new DateTime(_currentEnd.Year, _currentEnd.Month, _currentEnd.Day, _currentEnd.Hour, 0, 0);
                    Logger.Warn(String.Format("Fall back to region composer {0}", hour));

                    var query = new SinaSearch.WeiboSearchQuery()
                    {
                        AddtionQuery = _addtionQuery,
                        Keyword = _keyword,
                        isOrigin = _isOrigin,
                        isVip = _isVip,
                        StartPage = 1,
                        EndPage = 50,
                        Option = SearchWeiboOption.RealTime,
                        SearchAll = true,
                        StartDate = hour,
                        EndDate = hour,
                    };
                    _regionComposer.Initialize(query);

                    // 等区域算法结束之后从下一个小时开始继续跑
                    _currentEnd = hour.AddHours(-1);

                    return;
                }
                else
                {
                    _currentEnd = last.PubDate.Value;
                }
            }

            // 去重判定，可能没有必要
            result = result.Where(model => !_lastQueryItems.Contains(model.Url)).ToArray();
            _lastQueryItems.Clear();
            _lastQueryItems = new HashSet<String>(result.Select(model => model.Url));

            if (_currentEnd == _start)
            {
                _finished = true;
            }
        }
    }
}
