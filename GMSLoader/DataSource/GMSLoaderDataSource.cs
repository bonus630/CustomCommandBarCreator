using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Interop;
using System.Text;
using System.Windows.Controls;
using System.Linq;

namespace GMSLoader.DataSource
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GMSLoaderDataSource : BaseDataSource
    {

    
        private List<GMSProject> projects = new List<GMSProject>();
        private string path;

        public GMSLoaderDataSource(DataSourceProxy proxy) : base(proxy)
        {
            path = GetFolder();
        }




        private string GetNames(int index)
        {
            string result = string.Empty;
            using (FileStream fs = File.Open(string.Format("{0}/table.89", path), FileMode.Open, FileAccess.Read))
            {

                byte[] bytes = new byte[4];
                fs.Read(bytes, 0, 4);
                int itemsCount = BitConverter.ToInt32(bytes, 0);

                int position = 4;
                int itemPosition = 4 * itemsCount + position;

                for (int i = 0; i < index; i++)
                {
                    fs.Position = position;
                    fs.Read(bytes, 0, 4);
                    int itemSize = BitConverter.ToInt32(bytes, 0);
                    itemPosition += itemSize;
                    position += 4;
                }
                fs.Position = position;
                fs.Read(bytes, 0, 4);
                int size = BitConverter.ToInt32(bytes, 0);

                bytes = new byte[size];
                fs.Position = itemPosition;
                fs.Read(bytes, 0, size);

                result = Encoding.UTF8.GetString(bytes);

            }
            return result;
        }

        private string GetFolder()
        {
            string codeBase = typeof(GMSLoaderDataSource).Assembly.CodeBase;
            if (!string.IsNullOrEmpty(codeBase))
            {
                codeBase = codeBase.Remove(0, 8);
                codeBase = codeBase.Substring(0, codeBase.LastIndexOf("/"));
            }
            return Path.Combine(ControlUI.corelApp.AddonPath,codeBase);
        }

        private void Load(int index)
        {  
            if (!ControlUI.corelApp.InitializeVBA())
                return;
            string[] names = GetNames(index).Split('$');
            string path = string.Format("{0}\\{1}",GetFolder(), names[0]);

            string module = names[1].Substring(0, names[1].IndexOf("."));
            string macro = names[1].Replace(module + ".", "");



            GMSProject gmp = projects.SingleOrDefault(r=>r.FileName==names[0]);

            if (gmp == null)
            {
                gmp = ControlUI.corelApp.GMSManager.Projects.Load(path);
                projects.Add(gmp);
            }
            ControlUI.corelApp.GMSManager.RunMacro(module, macro);


        }
        public void UnloadGMS()
        {
            for (int i = 0; i < projects.Count; i++)
            {
                projects[i].Unload();
            }
            projects.Clear();
        }
        //Slots
        public void LoadGMS000() { Load(0);  }
        public void LoadGMS001() { Load(1);  }
        public void LoadGMS002() { Load(2);  }
        public void LoadGMS003() { Load(3);  }
        public void LoadGMS004() { Load(4);  }
        public void LoadGMS005() { Load(5);  }
        public void LoadGMS006() { Load(6);  }
        public void LoadGMS007() { Load(7);  }
        public void LoadGMS008() { Load(8);  }
        public void LoadGMS009() { Load(9);  }
        public void LoadGMS010() { Load(10); }
        public void LoadGMS011() { Load(11); }
        public void LoadGMS012() { Load(12); }
        public void LoadGMS013() { Load(13); }
        public void LoadGMS014() { Load(14); }
        public void LoadGMS015() { Load(15); }
        public void LoadGMS016() { Load(16); }
        public void LoadGMS017() { Load(17); }
        public void LoadGMS018() { Load(18); }
        public void LoadGMS019() { Load(19); }
        public void LoadGMS020() { Load(20); }
        public void LoadGMS021() { Load(21); }
        public void LoadGMS022() { Load(22); }
        public void LoadGMS023() { Load(23); }
        public void LoadGMS024() { Load(24); }
        public void LoadGMS025() { Load(25); }
        public void LoadGMS026() { Load(26); }
        public void LoadGMS027() { Load(27); }
        public void LoadGMS028() { Load(28); }
        public void LoadGMS029() { Load(29); }
        public void LoadGMS030() { Load(30); }
        public void LoadGMS031() { Load(31); }
        public void LoadGMS032() { Load(32); }
        public void LoadGMS033() { Load(33); }
        public void LoadGMS034() { Load(34); }
        public void LoadGMS035() { Load(35); }
        public void LoadGMS036() { Load(36); }
        public void LoadGMS037() { Load(37); }
        public void LoadGMS038() { Load(38); }
        public void LoadGMS039() { Load(39); }
        public void LoadGMS040() { Load(40); }
        public void LoadGMS041() { Load(41); }
        public void LoadGMS042() { Load(42); }
        public void LoadGMS043() { Load(43); }
        public void LoadGMS044() { Load(44); }
        public void LoadGMS045() { Load(45); }
        public void LoadGMS046() { Load(46); }
        public void LoadGMS047() { Load(47); }
        public void LoadGMS048() { Load(48); }
        public void LoadGMS049() { Load(49); }
        public void LoadGMS050() { Load(50); }
        public void LoadGMS051() { Load(51); }
        public void LoadGMS052() { Load(52); }
        public void LoadGMS053() { Load(53); }
        public void LoadGMS054() { Load(54); }
        public void LoadGMS055() { Load(55); }
        public void LoadGMS056() { Load(56); }
        public void LoadGMS057() { Load(57); }
        public void LoadGMS058() { Load(58); }
        public void LoadGMS059() { Load(59); }
        public void LoadGMS060() { Load(60); }
        public void LoadGMS061() { Load(61); }
        public void LoadGMS062() { Load(62); }
        public void LoadGMS063() { Load(63); }
        public void LoadGMS064() { Load(64); }
        public void LoadGMS065() { Load(65); }
        public void LoadGMS066() { Load(66); }
        public void LoadGMS067() { Load(67); }
        public void LoadGMS068() { Load(68); }
        public void LoadGMS069() { Load(69); }
        public void LoadGMS070() { Load(70); }
        public void LoadGMS071() { Load(71); }
        public void LoadGMS072() { Load(72); }
        public void LoadGMS073() { Load(73); }
        public void LoadGMS074() { Load(74); }
        public void LoadGMS075() { Load(75); }
        public void LoadGMS076() { Load(76); }
        public void LoadGMS077() { Load(77); }
        public void LoadGMS078() { Load(78); }
        public void LoadGMS079() { Load(79); }
        public void LoadGMS080() { Load(80); }
        public void LoadGMS081() { Load(81); }
        public void LoadGMS082() { Load(82); }
        public void LoadGMS083() { Load(83); }
        public void LoadGMS084() { Load(84); }
        public void LoadGMS085() { Load(85); }
        public void LoadGMS086() { Load(86); }
        public void LoadGMS087() { Load(87); }
        public void LoadGMS088() { Load(88); }
        public void LoadGMS089() { Load(89); }
        public void LoadGMS090() { Load(90); }
        public void LoadGMS091() { Load(91); }
        public void LoadGMS092() { Load(92); }
        public void LoadGMS093() { Load(93); }
        public void LoadGMS094() { Load(94); }
        public void LoadGMS095() { Load(95); }
        public void LoadGMS096() { Load(96); }
        public void LoadGMS097() { Load(97); }
        public void LoadGMS098() { Load(98); }
        public void LoadGMS099() { Load(99); }
        public void LoadGMS100() { Load(100); }
    }

}
