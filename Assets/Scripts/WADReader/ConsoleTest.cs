//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace WADReader
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var wadName = args[0];
//            var path = Path.Combine(Directory.GetCurrentDirectory(), wadName);

//            Console.WriteLine("Opening " + path);

//            var wadStream = new FileStream(path, FileMode.Open);

//            var wadInfo = WADReader.GetInfo(wadStream);

//            Console.Write(wadInfo);

//            Console.WriteLine("\n\nMaps\n----\n");

//            var maps = WADReader.GetMapList(wadStream, wadInfo);
//            foreach (var m in maps)
//                Console.WriteLine(m.ToString() + "\n");

//            var testMap = WADReader.GetMapData(wadStream, maps[0]);

//            Console.ReadLine();
//        }
//    }
//}
