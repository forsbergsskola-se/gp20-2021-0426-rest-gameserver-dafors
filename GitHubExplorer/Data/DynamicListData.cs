using System;
using System.Collections.Generic;

namespace GitHubExplorer.Data {
    public class DynamicListData : IUnmanagedData {
        private List<DynamicData> dynamicDatas;
        public DynamicListData(List<DynamicData> list) {
            this.dynamicDatas = list;
        }
        public void Print() {
            foreach (var dict in dynamicDatas) {
                dict.Print();
                Console.WriteLine();
            }
        }
    }
}