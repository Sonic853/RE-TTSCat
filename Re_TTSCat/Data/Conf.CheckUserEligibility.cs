﻿using BilibiliDM_PluginFramework;
using System.Text.RegularExpressions;
using System;
using Chinese;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckUserEligibility(string content)
        {
            switch (Vars.CurrentConf.BlockMode)
            {
                default:
                    return true;
                case 1:
                    var RegKeyWordArray_black = Vars.CurrentConf.BlackList.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string RegKeyWord in RegKeyWordArray_black)
                    {
                        // 如果内容有部分符合黑名单规则，就返回 false
                        Regex r = new Regex(RegKeyWord);
                        Match m = r.Match(content);
                        if (m.Success) return false;
                        if (content.Contains(RegKeyWord)) return false;
                        try
                        {
                            // 将文字转为拼音
                            var pContent = Pinyin.GetString(content, PinyinFormat.WithoutTone);
                            var pRegKeyWord = Pinyin.GetString(RegKeyWord, PinyinFormat.WithoutTone);
                            // 如果拼音有部分符合黑名单规则，就返回 false
                            r = new Regex(pRegKeyWord);
                            m = r.Match(pContent);
                            if (m.Success) return false;
                            if (pContent.Contains(pRegKeyWord)) return false;
                        }
                        catch (System.Exception e)
                        {
                            Bridge.ALog("转换拼音时出现错误：" + e.Message);
                        }
                    }
                    //return Vars.CurrentConf.BlackList.Contains(content) ? false : true;
                    return true;
                case 2:
                    var RegKeyWordArray_white = Vars.CurrentConf.WhiteList.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string RegKeyWord in RegKeyWordArray_white)
                    {
                        Regex r = new Regex(RegKeyWord);
                        Match m = r.Match(content);
                        if (m.Success) return true;
                        if (content.Contains(RegKeyWord)) return true;
                    }
                    //return Vars.CurrentConf.WhiteList.Contains(content) ? true : false;
                    return false;
            }
        }

        public static bool CheckUserEligibility(ReceivedDanmakuArgs e)
        {
            if (Vars.CurrentConf.BlockUID)
            {
                if (!CheckUserEligibility(e.Danmaku.UserID.ToString()))
                {
                    Bridge.ALog("忽略：用户已命中 UID 规则");
                    return false;
                }
            }
            else
            {
                if (!CheckUserEligibility(e.Danmaku.UserName))
                {
                    Bridge.ALog("忽略：用户已命中用户名规则");
                    return false;
                }
            }
            return true;
        }
    }
}
