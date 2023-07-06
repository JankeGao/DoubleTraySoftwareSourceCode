using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dtos
{
    public class WareHouseTree
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public List<ContainerDto> ContainerList { get; set; }
        public List<Entitys.Tray> TrayList { get; set; }
        public List<Entitys.Location> LocationList { get; set; }

        public List<Entitys.Area> AreaList { get; set; }
        public List<Entitys.Channel> ChannelList { get; set; }

    }
}
