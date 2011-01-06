using System;
using System.Collections.Generic;
using System.Text;

namespace ImhotepConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool convetWN = false;
            Console.WriteLine("Veuillez saisir l'étape de migration ?");
            string line = Console.ReadLine();
            #region AdExpress session
            switch (line)
            {
                case "1":
                    Console.WriteLine("Voulez vous migrer Websession => NewWebsession ?");
                    line = Console.ReadLine();
                    //Websession => NewWebsession
                    if (line == "y")
                    {
                         convetWN = ImhotepConsole.Rules.ImhotepRules.ConvertAllWebSessionToNewWebSession();
                    }
                    else
                    {
                        Console.WriteLine("Veuillez quitter l'application en appuyant sur la touche Entrée ");
                        Console.ReadLine();
                    }
                    break;
                case "2":
                    Console.WriteLine("Voulez vous migrer NewWebsession => Websession ?");
                    line = Console.ReadLine();
                    //NewWebsession => Websession
                    if (line == "y")
                    {
                         convetWN = ImhotepConsole.Rules.ImhotepRules.ConvertAllNewWebSessionToWebSession();
                    }
                    else
                    {
                        Console.WriteLine("Veuillez quitter l'application en appuyant sur la touche Entrée ");
                        Console.ReadLine();
                    }
                    break;
                case "3":
                    Console.WriteLine("Voulez vous migrer Alert Websession => NewWebsession ?");
                    line = Console.ReadLine();
                    //Alert Websession => NewWebsession
                    if (line == "y")
                    {
                         convetWN = ImhotepConsole.Rules.ImhotepRules.ConvertAllAlertWebSessionToNewWebSession();
                    }
                    else
                    {
                        Console.WriteLine("Veuillez quitter l'application en appuyant sur la touche Entrée ");
                        Console.ReadLine();
                    }
                    break;
                case "4":
                    Console.WriteLine("Voulez vous migrer Alert NewWebsession => Websession ?");
                    line = Console.ReadLine();
                    //Alert NewWebsession => Websession
                    if (line == "y")
                    {
                        convetWN = ImhotepConsole.Rules.ImhotepRules.ConvertAllAlertNewWebSessionToWebSession();
                    }
                    else
                    {
                        Console.WriteLine("Veuillez quitter l'application en appuyant sur la touche Entrée ");
                        Console.ReadLine();
                    }
                    break;
                default:
                    Console.WriteLine(" Votre saisie ne correspond à aucune étape de migration ");
                    Console.ReadLine();
                    break;
            }
          

          
            #endregion


           
           
            
            
        }
    }
}
