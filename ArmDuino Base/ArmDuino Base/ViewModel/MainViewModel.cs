using ArmDuino_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmDuino_Base;

namespace ArmDuino_Base.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current;
        public COMHandler COMHandler { get; set; }
        public static Arm Arm { get; set; }
        public SpokenCommand Picker { get; set; }
        public SpokenCommand Rect { get; set; }
        public SpokenCommand Salute { get; set; }
        public SpokenCommand Paja { get; set; }
        public SpokenCommand Navidades { get; set; }
        public SpokenCommand ParalaMusica { get; set; }
        public SpokenCommand Cumpleaños { get; set; }
        public MainViewModel()
        {
            Current = this;
            Arm = new Arm();
            COMHandler = new COMHandler(Arm);
            loadCommands();
        }

        public void loadCommands()
        {
            //Picker
            Picker = new SpokenCommand("Recogiendo");
            Picker.MovementQueue.Enqueue(new int[] { 90, 50, 90, 10, 90, 30, 160 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 50, 90, 10, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 90, 90, 30, 90, 50, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 90, 90, 30, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 0, 90, 90, 30, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 0, 90, 90, 30, 90, 30, 160 });
            //Rect
            Rect = new SpokenCommand("Firme");
            Rect.MovementQueue.Enqueue(new int[] { 90, 90, 90, 90, 90, 90, 170 });
            //Salute
            Salute = new SpokenCommand("Hola");
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 90, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 50, 30, 90, 30, 50 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 130, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 50, 30, 90, 30, 50 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 130, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 90, 90, 90, 90, 90, 170 });
            //Paja
            Paja = new SpokenCommand("Solo porque eres tú");
            Paja.MovementQueue.Enqueue(new int[] { 90, 60, 120, 30, 150, 90, 160 });
            for (int i = 0; i < 20; i++)
            {
                Paja.MovementQueue.Enqueue(new int[] { 90, 60, 120, 30, 150, 50, 140 });
                Paja.MovementQueue.Enqueue(new int[] { 70, 60, 120, 30, 150, 130, 140 });
            }
            //Navidades
            Navidades = new SpokenCommand("¡Feliz navidad!", "jingle");
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 70, 90, 70, 50, 50, 140 });
            Navidades.MovementQueue.Enqueue(new int[] { 90, 80, 90, 80, 130, 50, 170 });
            //Para la música
            ParalaMusica = new SpokenCommand("Jo, vale");
            ParalaMusica.MovementQueue.Enqueue(new int[] { 90, 70, 90, 30, 90, 20, 110});
            //Felicitar cumpleaños
            Cumpleaños = new SpokenCommand("¡Felicidades!", "HappyBirthdayGladOS");
            for (int i = 0; i < 20; i++)
            {
                Cumpleaños.MovementQueue.Enqueue(new int[] { 110, 70, 70, 120, 110, 70, 170 });
                Cumpleaños.MovementQueue.Enqueue(new int[] { 70, 120, 110, 70, 70, 120, 70});
            }
        }
    }
}
