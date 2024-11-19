using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ItemInfo {

        public string name { get; set; }

        public ItemInfo(string name) {
            this.name = name;
        }

    }

    [MemoryPackable]
    public partial class Dummy1ItemInfo : ItemInfo {
        public Dummy1ItemInfo(string name) : base(name) {

        }

    }

    [MemoryPackable]
    public partial class Dummy2ItemInfo : ItemInfo {
        public Dummy2ItemInfo(string name) : base(name) {

        }

    }

    [MemoryPackable]
    public partial class Dummy3ItemInfo : ItemInfo {
        public Dummy3ItemInfo(string name) : base(name) {

        }

    }

}
