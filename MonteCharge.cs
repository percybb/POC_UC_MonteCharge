using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UcMonteChange
{
    public partial class MonteCharge: UserControl
    {

        public delegate void MonteChargeDelegate(object obj, monteChargeEventArgs e);

        public event MonteChargeDelegate DoorIsOpen;

        private Timer timer,timerPriorite;
        private int operationMode;
        private int operationMode2;

        //Status precedent
        private int statusPrecedent = 0;


        //Status porte
        private const int OuverturePorte = 1;
        private const int FermeturePorte = 2;
        private int statusPorte = 0;
        //Status Charge
        private const int upCharge = 3;
        private const int downCharge = 4;
        private int StatuUpDown = 0;

        private int nbStage = 0;

        int sentido = 0; // 0 san , 1 = up, 2= down

        //status btnDemande 0 = sans Etat, 1 = up, 2 = down 
        private int upDownE2 = 0;
        private int upDownE1 = 0;
        private int upDownRC = 0;

        List<int> priorite = new List<int>();
        private int statusPriorite = 0;


        private int position=0;
        private int posRel = 0;
        private int posInitY=0;
        public MonteCharge()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            operationMode = 0;



            lumier(position);

            timerPriorite = new Timer();
            timerPriorite.Enabled = false;
            timerPriorite.Interval = 500;
            timerPriorite.Tick += TimerPriorite_Tick;

            starPriorite();

        }


        private void starPriorite()
        {
            timerPriorite.Enabled = true;
            timerPriorite.Start();
        }

        private void stopPriorite()
        {
            timerPriorite.Stop();
            timerPriorite.Enabled = false;
            
        }

        private void TimerPriorite_Tick(object sender, EventArgs e)
        {
            if(priorite.Count>0)
            {
                statusPriorite = 1;
                stopPriorite();
                int temp = priorite[0];
                if(temp> posRel)
                {
                    StatuUpDown = 1;
                }
                else
                {
                    StatuUpDown = 2;
                }   
                removePriorite(priorite, temp);
                action(temp);


            }
        }

        private void action(int nb)
        {
           
                if (position != nb)
                {
                    if ((nb - position) > 0)
                    {
                        operationMode2 = upCharge;
                        nbStage = Math.Abs(nb - position);
                        posInitY = panel2.Location.Y;
                        timer.Enabled = true;
                        timer.Start();
                    }
                    else if ((nb - position) < 0)
                    {
                        operationMode2 = downCharge;
                        nbStage = Math.Abs(nb - position);
                        posInitY = panel2.Location.Y;
                        timer.Enabled = true;
                        timer.Start();
                    }
                    else
                    {
                        MessageBox.Show("Vous etes la");
                        starPriorite();
                    }
                }
                else
                {
                    MessageBox.Show("Vous etes la");
                    starPriorite();

            }
            
        }


        [Browsable(true)]
        public int DoorTime
        {
            get { return this.timer.Interval; }
            set
            {
                this.timer.Interval = (value > 0) ? value : 500;
            }
        }

       


        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (operationMode2)
            {
                case upCharge:
                    upCharger(nbStage, posInitY);
                    break;
                case downCharge:
                    downCharger(nbStage, posInitY);
                    break;
                case OuverturePorte:
                    openDoor();
                    break;
                case FermeturePorte:
                    closeDoor();
                    break;
            }
        }

 
        private void openDoor()
        {
            if (panel4.Width > 10)
            {
                panel4.Width -= 10;
            }
            else
            {
                timer.Stop();
                timer.Enabled = false;
                statusPorte = 1;
                if (DoorIsOpen !=null)
                {
                    DoorIsOpen(this, new monteChargeEventArgs("La porte est ouvert"));
                    
                }

                switch (position)
                {
                    case 0:
                        btnRc.BackColor = Color.White;
                        break;
                    case 1:
                        btnE1.BackColor = Color.White;
                        break;
                    case 2:
                        btnE2.BackColor = Color.White;
                        break;
                    case 3:
                        btnE3.BackColor = Color.White;
                        break;
                }

                starPriorite();

            }
        }
    
        private void closeDoor()
        {
            if (panel4.Width <= 80)
            {
                panel4.Width += 10;
            }
            else
            {
                timer.Stop();
                timer.Enabled = false;
                statusPorte = 0;

                if(statusPrecedent!=0)
                {
                    operationMode2 = statusPrecedent;
                    timer.Enabled = true;
                    timer.Start();
                }
            }
        }

        private void btnOpenDoor_Click(object sender, EventArgs e)
        {
            if(operationMode2!= OuverturePorte)
            {
                operationMode2 = OuverturePorte;
                timer.Enabled = true;
                timer.Start();
                
            }
        }


        private void btnCloseDoor_Click(object sender, EventArgs e)
        {
            if (operationMode2 != FermeturePorte)
            {
                operationMode2 = FermeturePorte;
                timer.Enabled = true;
                timer.Start();                
            }
        }

        public void downCharger(int nb, int posInit)
        {   
            if(statusPorte==0)
            {          
                if (panel2.Location.Y < (posInit + (100*nb) ))
                {
                    StatuUpDown = 2;
                    int temp = this.panel2.Location.Y + 10;               
                    panel2.Location = new Point(panel2.Location.X, panel2.Location.Y + 10);
                    if (sensorPos(temp) != 4)
                    {
                        lumier(sensorPos(temp));
                        posRel = sensorPos(temp);
                    }
                }
                else
                {
                    position -= nb;
                    lumier(position);                    
                    timer.Stop();
                    timer.Enabled = false;
                    StatuUpDown = 0;

                    operationMode2 = OuverturePorte;
                    timer.Enabled = true;
                    timer.Start();

                }
            }
            else
            {
                statusPrecedent = downCharge;
                operationMode2 = FermeturePorte;
                timer.Enabled = true;
                timer.Start();
            }
        }

        public void upCharger(int nb, int posInit)
        {
            if(statusPorte==0)
            {          
                if (panel2.Location.Y > (posInit - (100*nb)))
                {
                    StatuUpDown = 1;
                    int temp = this.panel2.Location.Y - 10;            
                    this.panel2.Location=new Point(this.panel2.Location.X,temp);              

                    if(sensorPos(temp)!=4)
                    {
                        lumier(sensorPos(temp));
                        posRel = sensorPos(temp);
                    }    
                }
                else
                {               
                    position += nb;
                    lumier(position);                  
                    timer.Stop();
                    timer.Enabled = false;
                    StatuUpDown = 0;

                    operationMode2 = OuverturePorte;
                    timer.Enabled = true;
                    timer.Start();
                }
            }
            else
            {
                statusPrecedent = upCharge;
                    operationMode2 = FermeturePorte;
                    timer.Enabled = true;
                    timer.Start();                
            }
        }

        private void btnRc_Click(object sender, EventArgs e)
        {
            // action(0);
            if (btnRc.BackColor != Color.SkyBlue)
            {
                btnRc.BackColor = Color.SkyBlue;
            }
            addPriorite(priorite, 0);
            ordenerPriorite(priorite);
        }

        private void btnE1_Click(object sender, EventArgs e)
        {
            //  action(1);
            if (btnE1.BackColor != Color.SkyBlue)
            {
                btnE1.BackColor = Color.SkyBlue;
            }
            addPriorite(priorite, 1);
            ordenerPriorite(priorite);
        }
        private void btnE2_Click(object sender, EventArgs e)
        {
            // action(2);
            if (btnE2.BackColor != Color.SkyBlue)
            {
                btnE2.BackColor = Color.SkyBlue;
            }
            addPriorite(priorite, 2);
            ordenerPriorite(priorite);
        }

        private void btnE3_Click(object sender, EventArgs e)
        {
            //action(3);
            if (btnE3.BackColor != Color.SkyBlue)
            {
                btnE3.BackColor = Color.SkyBlue;
            }
            addPriorite(priorite, 3);
            ordenerPriorite(priorite);
        }

        private void lumier(int etage)
        {
            switch (etage)
            {
                case 0:
                    lblE3.BackColor = Color.White;
                    lblE2.BackColor = Color.White;
                    lblE1.BackColor = Color.White;                    
                    lblRC.BackColor = Color.Green;
                    break;
                case 1:
                    lblE3.BackColor = Color.White;
                    lblE2.BackColor = Color.White;
                    lblE1.BackColor = Color.Green;                    
                    lblRC.BackColor = Color.White;
                    break;
                case 2:
                    lblE3.BackColor = Color.White;
                    lblE2.BackColor = Color.Green;
                    lblE1.BackColor = Color.White;                    
                    lblRC.BackColor = Color.White;
                    break;
                case 3:
                    lblE3.BackColor = Color.Green;
                    lblE2.BackColor = Color.White;
                    lblE1.BackColor = Color.White;                    
                    lblRC.BackColor = Color.White;
                    break;               
            }
        }

        private void btnDown2E_Click(object sender, EventArgs e)
        {
            if (btnDown2E.BackColor == Color.SkyBlue)
            {
                btnDown2E.BackColor = Color.White;
            }
            else
            {
                btnDown2E.BackColor = Color.SkyBlue;
               //addPriorite(priorite, "2ED");

            }
        }

        private void btnUp1E_Click(object sender, EventArgs e)
        {
            if (btnUp1E.BackColor == Color.SkyBlue)
            {
                btnUp1E.BackColor = Color.White;
            }
            else
            {
                btnUp1E.BackColor = Color.SkyBlue;
               // addPriorite(priorite, "1EU");

            }
        }

        private void brnDown1E_Click(object sender, EventArgs e)
        {
            if (brnDown1E.BackColor == Color.SkyBlue)
            {
                brnDown1E.BackColor = Color.White;
            }
            else
            {
                brnDown1E.BackColor = Color.SkyBlue;
                //addPriorite(priorite, "1ED");
            }
        }

        private void btnUpRC_Click(object sender, EventArgs e)
        {
            if (btnUpRC.BackColor == Color.SkyBlue)
            {
                btnUpRC.BackColor = Color.White;
            }
            else
            {
                btnUpRC.BackColor = Color.SkyBlue;
                //addPriorite(priorite, "0EU");
            }
        }

        private int sensorPos(int pos)
        {
            int etage = 0;
            if((0 < pos) && (pos <30))
            {
                etage = 3;
            }
            else if ((90 < pos) && (pos < 120))
            {
                etage = 2;
            }
            else if ((190 < pos) && (pos < 220))
            {
                etage = 1;
            }
            else if ((290 < pos) && (pos < 320))
            {
                etage = 0;
            }
            else
            {
                etage = 4;
            }
            return etage;
        }

        private void addPriorite(List<int> tab,int val)
        {
            //int len = tab.Count;
            if(!tab.Contains(val))
            {
                tab.Add(val);
            }
        }

       

        private void removePriorite(List<int> tab, int val)
        {
            //int len = tab.Count;
            if (tab.Contains(val))
            {
                tab.Remove(val);
            }
        }

        private void ordenerPriorite(List<int> tab)
        {
            List<int> tempTab = new List<int>();
            List<int> tempTab1 = new List<int>();
            Console.WriteLine(tab);
            if (tab.Count>1)
            {
                if(StatuUpDown == 1)
                {
                    foreach(int i in tab)
                    {
                        if(i> posRel)
                        {
                            //removePriorite(tab, i);
                            tempTab.Add(i);
                        }
                        else
                        {
                            tempTab1.Add(i);
                        }
                       
                    }
                    tempTab.Sort();

                    tab.Clear();
                    tab.AddRange(tempTab);
                    tab.AddRange(tempTab1);

                    //for (int j = tempTab.Count-1; j>=0; j--)
                    //{
                    //    tab.Insert(0, tempTab[j]);
                    //}    
                }

                if (StatuUpDown == 2)
                {
                    foreach (int i in tab)
                    {
                        if (i < posRel)
                        {
                            //removePriorite(tab, i);
                            tempTab.Add(i);
                        }
                        else
                        {
                            tempTab1.Add(i);
                        }

                    }

                   

                    tab.Clear();
                    tab.AddRange(tempTab.OrderByDescending(x => x).ToList());
                    tab.AddRange(tempTab1);

                    //for (int j = 0; j< tempTab.Count; j++)
                    //{
                    //    tab.Insert(0, tempTab[j]);
                    //}
                }
            }
        }



    }
}
