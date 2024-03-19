using AngleSharp.Dom;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sacar_Hilos_Fino
{
    public class Resultado
    {
        public string titulo { get; set; }
        public string URL { get; set; }

        public string tipo { get; set; } = "POST";



        public Resultado(string title, string link, string kind = "POST")
        {
            titulo = title;
            URL = link;
            tipo = kind;

        }
    }
}
