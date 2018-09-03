/* 
 * 根据加载规律缓存资源。
 * 每种规律有自己的判断方法
 */

using System;
using System.Collections.Generic;

namespace PackReader
{
    public class MyCache
    {

        public static MyCache instance;

        public static MyCache GetInstance()
        {
            if (instance==null)
            {
                instance = new MyCache();
            }
            return instance;
        }

        //每当有新规律添加时，在构造函数中将规律的判断方法加入。
        public MyCache()
        {
            rules.Add(Rule1Repeat);

            //rules.Add(Rule2);
        }

        //保存所有规律的判断方法
        private delegate bool Rule(string name, byte[] file);
        private List<Rule> rules = new List<Rule>();

        //保存所有规律的缓存文件
        private Dictionary<string, byte[]> allCacheFiles = new Dictionary<string, byte[]>();
        //private List<Dictionary<string, byte[]>> allCacheFiles;

        //缓存文件和规律序号绑定。
        //避免多种规律同时指定了同一缓存文件时产生的问题：
        //1.为了保证缓存区不要有重复的文件。2.只有当所有规律都不符合该文件时，才从缓存区删除此文件。
        private Dictionary<string, List<int>> cacheRule = new Dictionary<string, List<int>>();

        #region 规律变量

        private int r1Count;        //同一个文件被连续加载的次数
        private string r1Name;      //记录上一次加载的文件

        //private int r2Count;
        //private string r2Name;

        #endregion

        //检测是否符合规律。
        public bool RuleDetection(string name, byte[] file)
        {
            foreach (Rule rule in rules)
            {
                return rule(name, file);
            }
            return false;
        }

        //从缓存区查找文件
        public byte[] GetCacheFile(string name)
        {
            if (allCacheFiles.ContainsKey(name))
            {
                return allCacheFiles[name];
            }
            return null;
        }

        //添加到缓存区
        private void AddCache(string name,byte[] file,int ruleIndex)
        {
            //if (cacheFiles.ContainsKey(name)) return;
            //cacheFiles.Add(name, file);

            //记录当前文件和哪个规律相关联
            if (cacheRule.ContainsKey(name))
            {
                List<int> ruleList = cacheRule[name];
                if(!ruleList.Contains(ruleIndex))
                    ruleList.Add(ruleIndex);
            }
            else
            {
                List<int> ruleList = new List<int>();
                ruleList.Add(ruleIndex);
                cacheRule.Add(name, ruleList);
            }

            //添加进缓存区
            if (!allCacheFiles.ContainsKey(name))
                allCacheFiles.Add(name, file);
        }

        //清理缓存区
        private void ClearCache(string name,int ruleIndex)
        {
            //cacheFiles.Clear();
            //移除当前文件和规律的关联，如果该文件没有和任何一个规律有关联，则从缓存区中移除
            if (cacheRule.ContainsKey(name))
            {
                List<int> ruleList = cacheRule[name];
                if (ruleList.Contains(ruleIndex))
                    ruleList.Remove(ruleIndex);
                if (ruleList.Count == 0)
                    allCacheFiles.Remove(name);
            }

        }

        //重复加载同一资源的规律: a,a,a,a....
        private bool Rule1Repeat(string name,byte[] file)
        {
            if (r1Name == null)
            {
                r1Name = name;
            }
            //符合重复加载规律则增加计数，否则清除缓存
            if (r1Name == name)
            {
                r1Count++;
            }
            else
            {
                //上一个缓存文件清除
                ClearCache(r1Name, 1);
                r1Name = name;
                r1Count = 1;
                return false;
            }

            //重复加载一个文件2次以上，则把当前文件加入缓存区
            if (r1Count>=2)
            {
                AddCache(name, file,1);
                return true;
            }

            return false;

        }


        /*
        //规律：a,b,a,b,a,b.....
        private bool Rule2Temp(string name,byte[] file)
        {
            return false;
        }
        */

    }
}
