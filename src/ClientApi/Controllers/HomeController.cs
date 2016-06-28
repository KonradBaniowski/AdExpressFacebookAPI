﻿using Facebook.Service.Contract.BusinessModels;
using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ClientApi.Controllers
{
    public class HomeController : Controller
    {

        private IRightService _rightSvc;
        private IFacebookPageService _fbsvc;
        private IProductService _productSvc;

        public HomeController(IRightService rightSvc, IFacebookPageService fbsvc, IProductService productSvc)
        {
            _rightSvc = rightSvc;
            _fbsvc = fbsvc;
            _productSvc = productSvc;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Test()
        {
            //var dbfb = _fbsvc.GetDataFacebook();
            //var products = _productSvc.GetLevelItems("BOISSONS",1);
            //var test2 = _rightSvc.GetProductRight(1087);
            //var next = _rightSvc.GetMediaRight(1087);
            //var test2 = _rightSvc.GetProductRight(1084);
            //var next = _rightSvc.GetMediaRight(1084);            
            return View("Index");
        }


        public ActionResult Rights()
        {
            //List<int> logs = new List<int>
            //{
            // 1047,1084,1085,1086,1087,1088,1089,1091,1093,1094,1102,1105,1115,1145,1155,1220,1272,1300,1310,1311,1320,1331,1360,1400,1410,1421,1430,1440,1490,1501,1540,1589,1610,1631,1670,1680,1701,1710,1781,1810,1830,1840,1852,1855,1861,1880,1890,1901,1902,1908,1920,1980,1990,2010,2042,2043,2044,2061,2134,2153,2163,2174,2185,2186,2193,2213,2214,2215,2216,2217,2218,2219,2223,2224,2225,2256,2257,2258,2259,2277,2278,2341,2342,2343,2344,2345,2357,2358,2427,2437,2448,2457,2471,2518,2519,2547,2577,2587,2597,2607,2608,2627,2669,2679,2701,2738,2742,2748,2768,2788,2800,2808,2809,2828,2858,2888,2898,2917,2928,2929,2950,2951,2958,2988,2998,3008,3028,3030,3069,3070,3078,3148,3149,3159,3208,3228,3240,3248,3299,3309,3349,3369,3380,3399,3409,3420,3429,3439,3449,3469,3479,3482,3492,3581,3591,3629,3639,3659,3759,3779,3799,3829,3830,3873,3877,3890,3899,3906,3909,3913,3918,3929,3938,3958,3959,3968,3999,4038,4088,4098,4109,4110,4139,4179,4180,4188,4198,4220,4240,4241,4243,4244,4251,4280,4301,4309,4340,4359,4362,4363,4380,4381,4399,4400,4511,4545,4582,4592,4603,4623,4682,4712,4752,4762,4802,4803,4807,4808,4812,4814,4815,4816,4822,4827,4862,4952,5003,5012,5022,5023,5024,5025,5026,5027,5028,5029,5030,5031,5032,5033,5034,5035,5036,5037,5038,5039,5040,5041,5052,5062,5093,5094,5095,5096,5097,5098,5102,5103,5104,5112,5122,5132,5133,5134,5135,5136,5137,5140,5154,5164,5188,5189,5190,5198,5218,5248,5258,5280,5281,5282,5283,5298,5309,5310,5329,5330,5339,5340,5341,5369,5370,5379,5392,5399,5409,5420,5431,5439,5440,5460,5470,5482,5503,5504,5506,5513,5523,5608,5629,5631,5688,5708,5724,5725,5734,5835,5836,5846,5857,5886,5913,5923,5944,5994,6016,6027,6037,6047,6139,6190,6222,6230,6242,6243,6300,6310,6322,6323,6324,6330,6331,6333,6360,6370,6430,6521,6531,6571,6581,6591,6601,6602,6612,6672,6682,6692,6732,6796,6797,6798,6932,6942,7004,7032,7122,7172,7232,7233,7314,7342,7382,7452,7513,7534,7538,7539,7543,7562,7566,7570,7572,7625,7642,7652,7662,7713,7732,7742,7763,7773,7803,7835,7836,7872,7893,7903,7906,7980,7992,8052,8053,8072,8073,8074,8095,8099,8108,8110,8111,8112,8113,8122,8124,8125,8126,8127,8128,8140,8142,8152,8264,8303,8322,8363,8444,8452,8453,8454,8474,8542,8553,8555,8583,8584,8592,8602,8622,8662,8733,8742,8743,8744,8745,8746,8747,8762,8792,8803,8822,8832,8842,8852,8883,8902,8913,8918,8962,8972,8982,9012,9052,9053,9054,9055,9056,9082,9083,9084,9092,9134,9162,9213,9262,9263,9272,9273,9274,9275,9276,9282,9303,9332,9482,9492,9513,9594,9612,9682,9702,9762,9774,9803,9824,9825,9852,9903,9923,9942,9952,10144,10145,10146,10154,10155,10172,10264,10273,10303,10343,10382,10392,10393,10402,10403,10404,10405,10406,10407,10408,10409,10410,10432,10455,10456,10457,10460,10482,10503,10504,10512,10513,10514,10533,10534,10543,10544,10552,10562,10563,10564,10565,10566,10567,10568,10569,10570,10571,10572,10573,10574,10575,10576,10577,10578,10579,10580,10581,10582,10583,10584,10585,10586,10587,10588,10589,10590,10591,10592,10593,10594,10595,10596,10597,10598,10599,10600,10601,10602,10603,10604,10605,10606,10607,10608,10609,10610,10612,10614,10623,10632,10633,10645,10664,10674,10675,10676,10677,10679,10681,10683,10685,10692,10693,10694,10702,10742,10762,10832,10834,10853,10854,10855,10856,10902,10934,10982,10992,10993,10994,10995,10996,11002,11043,11045,11052,11062,11063,11064,11066,11071,11092,11123,11142,11222,11234,11235,11236,11237,11262,11312,11342,11364,11365,11372,11402,11415,11422,11442,11483,11562,11592,11623,11624,11682,11693,11712,11724,11753,11784,11792,11793,11802,11804,11853,11862,11872,11902,11912,11972,11973,11992,12022,12072,12102,12112,12132,12162,12173,12222,12242,12243,12292,12302,12312,12314,12315,12322,12323,12326,12342,12352,12393,12402,12412,12433,12434,12442,12463,12512,12562,12573,12585,12586,12587,12613,12614,12622,12632,12652,12663,12664,12667,12669,12670,12671,12673,12674,12692,12702,12722,12764,12832,12833,12852,12854,12872,12882,12902,12912,12925,13002,13012,13023,13089,13122,13123,13126,13142,13157,13162,13174,13175,13194,13195,13232,13242,13262,13296,13297,13298,13346,13366,13367,13368,13376,13377,13378,13392,13396,13406,13407,13408,13417,13426,13468,13470,13480,13491,13510,13521,13522,13524,13525,13526,13527,13529,13530,13532,13550,13560,13580,13590,13591,13601,13610,13620,13630,13641,13643,13647,13670,13672,13674,13684,13686,13701,13731,13750,13760,13761,13780,13781,13790,13800,13802,13810,13838,13840,13870,13930,13960,13961,13971,13990,14010,14041,14050,14070,14090,14091,14141,14160,14170,14190,14220,14230,14240,14260,14325,14361,14370,14391,14433,14435,14444,14454,14466,14510,14547,14548,14549,14560,14590,14622,14630,14631,14632,14633,14650,14651,14660,14671,14673,14680,14681,14750,14760,14780,14781,14782,14783,14784,14790,14791,14810,14820,14831,14840,14870,14890,14891,14900,14920,14930,14931,14932,14950,14963,14964,14970,14971,15030,15031,15033,15040,15070,15080,15100,15130,15150,15170,15190,15220,15230,15240,15260,15290,15440,15460,15471,15490,15500,15515,15523,15541,15563,15573,15574,15600,15611,15612,15613,15614,15630,15640,15650,15661,15670,15691,15700,15720,15721,15750,15760,15770,15790,15800,15840,15841,15842,15843,15900,15910,15920,15950,15951,16010,16020,16040,16050,16060,16070,16080,16081,16090,16091,16092,16100,16110,16120,16180,16200,16220,16230,16240,16250,16260,16280,16290,16300,16316,16332,16333,16334,16335,16336,16337,16338,16340,16350,16360,16370,16373,16374,16380,16390,16391,16410,16420,16430,16460,16470,16480,16490,16500,16520,16530,16540,16550,16580,16610,16620,16630,16640,16641,16650,16660,16661,16670,16680,16690,16700,16720,16740,16750,16760,16770,16780,16781,16790,16800,16810,16820,16830,16831,16832,16850,16880,16901,16910,16920,16930,16940,16950,16960,16961,16962,16970,16971,16980,16981,16982,16983,16984,16985,16990,17000,17010,17020,17030,17040,17050,17060,17070,17080,17081,17090,17100,17110,17120,17130,17140,17141,17150,17160,17170,17180,17190,17200,17201,17210,17220,17230,17231,17240,17250,17270,17280,17290,17310,17311,17312,17313,17314,17315,17316,17320,17350,17370,17380,17390,17400,17410,17420,17430,17450,17460,17480,17490,17510,17540,17550,17570,17571,17581,17590,17591,17592,17593,17594,17600,17610,17611,17612,17620,17630,17640,17660,17670,17680,17700,17701,17702,17770,17792,17950,17960,18010,18011,18020,18030,18040,18050,18051,18060,18070,18080,18090,18100,18110,18120,18130,18140,18150,18160,18161,18162,18163,18164,18165,18166,18170,18190,18200,18210,18212,18220,18230,18240,18241,18242,18243,18244,18250,18260,18270,18280,18290,18300,18320,18350,18351,18370,18390,18400,18401,18402,18403,18404,18405,18406,18407,18408,18409,18410,18411,18412,18413,18414,18415,18416,18417,18418,18419,18420,18421,18422,18423,18424,18425,18426,18427,18428,18429,18430,18431,18432,18433,18434,18435,18436,18437,18438,18439,18440,18441,18442,18443,18444,18445,18446,18447,18448,18449,18450,18451,18452,18453,18454,18455,18456,18457,18458,18459,18460,18461,18462,18463,18464,18465,18466,18467,18468,18469,18470,18471,18472,18473,18474,18475,18476,18477,18478,18479,18480,18481,18482,18483,18484,18485,18486,18487,18488,18489,18490,18491,18492,18493,18494,18495,18496,18497,18498,18499,18500,18501,18502,18503,18504,18505,18506,18507,18508,18509,18510,18511,18512,18513,18514,18515,18516,18517,18518,18519,18520,18521,18522,18523,18524,18525,18526,18527,18528,18529,18530,18531,18532,18533,18534,18535,18536,18537,18538,18539,18540,18541,18542,18543,18544,18545,18546,18547,18548,18549,18550,18551,18552,18553,18554,18555,18556,18557,18558,18559,18560,18561,18562,18563,18564,18565,18566,18567,18568,18569,18570,18571,18572,18573,18574,18575,18576,18577,18578,18579,18580,18581,18582,18583,18584,18585,18586,18587,18588,18589,18590,18591,18592,18593,18594,18595,18596,18597,18598,18599,18600,18601,18602,18603,18604,18605,18606,18607,18608,18609,18610,18611,18612,18613,18614,18615,18616,18617,18618,18619,18620,18621,18622,18623,18624,18625,18626,18627,18628,18629,18630,18631,18632,18633,18634,18635,18636,18637,18638,18639,18640,18641,18642,18643,18644,18645,18646,18647,18648,18649,18650,18651,18652,18653,18654,18655,18656,18657,18658,18659,18660,18661,18662,18663,18664,18665,18666,18667,18668,18669,18670,18671,18672,18673,18674,18675,18676,18677,18678,18679,18680,18681,18682,18683,18684,18685,18686,18687,18688,18689,18690,18691,18692,18693,18694,18695,18696,18697,18698,18699,18700,18701,18702,18703,18704,18705,18706,18707,18708,18709,18710,18711,18712,18713,18714,18715,18716,18717,18718,18719,18720,18721,18722,18723,18724,18725,18726,18727,18728,18729,18730,18731,18732,18733,18734,18735,18736,18737,18738,18739,18740,18741,18742,18743,18744,18745,18746,18747,18748,18749,18750,18751,18752,18753,18754,18755,18756,18757,18758,18759,18760,18761,18762,18763,18764,18765,18766,18767,18768,18769,18770,18771,18772,18773,18774,18775,18776,18777,18778,18779,18780,18800,18811,18820,18830,18840,18860,18870,18880,18890,18891,18920,18930,18931,18940,18950,18960,18970,18981,18990,19000,19010,19020,19030,19040,19080,19101,19110,19120,19121,19122,19123,19130,19140,19150,19160,19171,19180,19190,19200,19210,19220,19221,19240,19241,19242,19243,19244,19245,19250,19260,19280,19290,19300,19310,19320,19330,19350,19360,19390,19400,19410,19420,19430,19440,19460,19470,19480,19490,19500,19520,19530,19550,19560,19570,19580,19590,19600,19610,19620,19630,19650,19660,19670,19671,19680,19681,19682,19683,19684,19690,19691,19692,19700,19710,19730,19740,19750,19760,19770,19780,19790,19800,19810,19811,19820,19830,19840,19850,19851,19852,19853,19854,19855,19856,19857,19860,19861,19862,19863,19864,19865,19866,19870,19880,19890,19900,19902,19910,19920,19930,19943,19950,19960,19970,19980,19990,20000,20001,20002,20010,20022,20030,20040,20050,20060,20070,20080,20100,20110,20120,20140,20150,20160,20200,20201,20220,20280,20300,20320,20321,20330,20340,20390,20400,20420,20421,20431,20432,20433,20434,20435,20436,20437,20438,20439,20440,20441,20442,20443,20444,20445,20446,20447,20448,20449,20450,20451,20452,20453,20454,20455,20456,20457,20458,20459,20460,20461,20462,20463,20464,20465,20466,20467,20468,20469,20470,20471,20472,20474,20475,20476,20477,20478,20479,20480,20490,20491,20500,20501,20502,20503,20504,20510,20511,20512,20530,20540,20570,20590,20600,20601,20602,20603,20604,20605,20606,20607,20608,20609,20615,20620,20621,20630,20632,20633,20634,20635,20636,20640,20651,20660,20680,20690,20691,20700,20710,20711,20713,20720,20730,20740,20750,20751,20761,20770,20780,20790,20800,20842,20850,20862,20870,20880,20890,20891,20892,20893,20894,20895,20897,20910,20930,20931,20932,20942,20950,20960,20980,20990,21002,21020,21030,21040,21050,21070,21082,21100,21121
            //};
            //foreach (var e in logs)
            //{
            //    var OTM = _rightSvc.GetTemplateMediaRight(e);
            //    var OCM = _rightSvc.GetMediaRight(e);
            //    var OTP = _rightSvc.GetTemplateProductRight(e);
            //    var OCP = _rightSvc.GetProductRight(e);
            //}

            //var OTM = _rightSvc.GetTemplateMediaRight(2457);
            //var OCM = _rightSvc.GetMediaRight(2457);
            //var OTP = _rightSvc.GetTemplateProductRight(2457);
            //var OCP = _rightSvc.GetProductRight(2457);
            //OTM.AddRange(OCM);
            //OTM.AddRange(OTP);
            //OTM.AddRange(OCP);

            //List<Criteria> filter = OTM;
            long begin = 20150101;
            long end = 20160301;

            var model = _fbsvc.GetDataFacebook(1155, begin, end, new List<long> {1060, 332860,48750 }, null);
            
            return View("Index");
        }
    }
}
