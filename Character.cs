using System.Net.Mail;
namespace Rougelike
{
    internal class Character
    {
        internal string name;   // Character name
        internal charRole role; // Character role
        internal int[] attr;    //Character attributes: 0 STR, 1 DEX, 2 END, 3 WILL, 4 INT, 5 WIS, 6 LUCK
        internal int[] HP = new int[2]; //Character Health: 0 current, 1 max
        internal int DR; //DR = Defense Rating
        internal bool[] boost_type = new bool[7];   // Active boost types
        internal int[] boost_value = new int[7], boost_timer=new int[7];    // Timer for boost types

        internal Character(string name, charRole role, int[] attr) {    // Creating new Character
            this.name = name;
            this.role = role;
            this.attr = attr;
            if(this.name.Equals("Enemy")) this.DR = Math.Max(attr[2], attr[1])/10;
            else this.DR = 5+Math.Max(attr[2], attr[1])/5;
            setStats();
            this.HP[0] = this.HP[1];
        }

        internal static Character newChar() // Creating new player Character
        {
            Console.WriteLine("What is the name of the character?");
            string name = System.Console.ReadLine() ?? "";
            
            System.Console.WriteLine("What is the role of "+name+"?");
            System.Console.WriteLine("0: Adventurer | 1: Thief | 2: Archer \n3: Warrior | 4: Wizard | 5: Priest");
            charRole role = parseRole(System.Console.ReadLine() ?? "0");

            System.Console.WriteLine("How do you want to generate attributes of "+name+"?");
            System.Console.WriteLine("r: Random | c: Choose");
            int[] attr = newAttr(System.Console.ReadLine() ?? "r");

            return new Character(name, role, attr);
        }
        internal static Character newEnemy(){   // Creating new enemy character
            return new Character("Enemy", parseRole("-1"), newAttr("enemy"));
        }
        internal static int[] newAttr(string parse){    // Genereting attribute values for Character
            int[] attr = new int[7];

            switch (parse) {
                case "r":
                case "Random":
                    for(int i = 0; i < attr.Length; i++){
                        attr[i] = Program.roll(3, 6);
                    }
                    break;
                case "c":
                case "Choose":
                    System.Console.WriteLine("Level of Strength? (3-18)");
                    attr[0] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Dexterity? (3-18)");
                    attr[1] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Endurance? (3-18)");
                    attr[2] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Will? (3-18)");
                    attr[3] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Intelligence? (3-18)");
                    attr[4] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Wisdom? (3-18)");
                    attr[5] = int.Parse(System.Console.ReadLine() ?? "3");

                    System.Console.WriteLine("Level of Luck? (3-18)");
                    attr[6] = int.Parse(System.Console.ReadLine() ?? "3");
                    break;
                case "enemy":
                    for(int i = 0; i < attr.Length; i++){
                        attr[i] = Program.roll((int)Dungeon.danger, (int)(Dungeon.danger*3), ((int)Math.Max((Dungeon.danger/2),1)));
                    }
                    break;
            }
            return attr;
        }
        internal static charRole parseRole(String parse){   // Converting string text to enum of charRole
            charRole role;
            if (parse == "-1"){
                parse = (Program.roll(1,5,0)).ToString();
            } else if (parse.Length == 1){
                parse = Enum.GetName(typeof(charRole), value: int.Parse(parse)) ?? "Adventurer";
            }
            Enum.TryParse(parse, out role);

            return role;
        }
        internal static attribute parseAttribute(String parse){ // Converting string text to enum of attribute
            attribute attribute;
            if (parse == "-1"){
                parse = (Program.roll(1,5,0)).ToString();
            } else if (parse.Length == 1){
                parse = Enum.GetName(typeof(attribute), value: int.Parse(parse)) ?? "Strength";
            }
            Enum.TryParse(parse, out attribute);

            return attribute;
        }
        
        internal void setStats() {  // Setting health max value
            switch(role) {
                case charRole.Adventurer:
                    this.HP[1] = attr[2]*5;
                    break;
                case charRole.Thief:
                    this.HP[1] = (int) Math.Floor((attr[0]+attr[2])*1.5);
                    break;
                case charRole.Archer:
                    this.HP[1] = (int) Math.Floor(attr[2]*2.5+attr[0]);
                    break;
                case charRole.Warrior:
                    this.HP[1] = (int) Math.Floor(attr[0]*2.5+attr[2]*3.5);
                    break;
                case charRole.Wizard:
                    this.HP[1] = (int) Math.Floor((attr[0]+attr[2]+attr[3])*0.75);
                    break;
                case charRole.Priest:
                    this.HP[1] = (int) Math.Floor((attr[0]+attr[3])*0.75+attr[2]);
                    break;
            }

        }
        public int baseAttack(Character taretChar){ // Function for basic attack 
            int acc = 0, damage = 0;
            switch (this.role){
                case charRole.Adventurer:
                case charRole.Warrior:
                    acc = this.attr[1]/3+Program.roll(2, 6);
                    damage = this.attr[0]+Program.roll(3, 4);
                    break;
                case charRole.Archer:
                case charRole.Thief:
                    acc = Program.roll(this.attr[1]/5+1, 6);
                    damage = this.attr[0]+Program.roll(this.attr[1]/5+1, this.attr[0]/5+1);
                    break;
                case charRole.Priest:
                    acc = this.attr[1]+Program.roll(this.attr[5]/5, 6);
                    damage = Program.roll(3, 3);
                    break;
                case charRole.Wizard:
                    acc = this.attr[1]+Program.roll(this.attr[4]/5, 6);
                    damage = Program.roll(3, 3);
                    break;
            }
            System.Console.WriteLine(this.name+" is trying to attack "+taretChar.name+" with basic attack!");
            if (acc >= taretChar.DR){
                System.Console.WriteLine(this.name+" hits the target and delivers "+damage+" damage!");
                taretChar.takeDamage(damage); 
                return 1;
            }else{
                System.Console.WriteLine(this.name+" misses!");
                return 0;
            }
        }
        public int bonus(int attribute){    // Function for calculating bonus of an attribute
            return (attr[attribute]-10)/2;
        }
        internal void takeDamage(int damage){   // Function for damaging a Character
            if(boost_type[0]) damage -= this.DR;
            if(damage > 0)this.HP[0]-=damage;
            else System.Console.WriteLine(this.name+" has blocked all damage!");

            if(this.HP[0] <= 0){
                string s = this.name+" died!";
                if(!this.name.Equals("Enemy")){
                    System.Console.WriteLine(s+" GAME OVER!");
                    Program.exit(exitScore());
                }
                System.Console.WriteLine(s);
            }else{
                System.Console.WriteLine(this.name+" has taken "+damage+" points of damage! Health: "+this.HP[0]+"/"+this.HP[1]);
            }
        }

        internal float exitScore() {    // Calculating final score
            float score = 0;
            foreach (var att in attr) {
                score+=att;
            }
            score+=Dungeon.room;
            score = score*Dungeon.danger/10;
            return score;
        }

        internal void restoreHealth(int heal){  // Function for restoring Characters health
            this.HP[0]+=heal;
            if(this.HP[0]>this.HP[1]) this.HP[0] = this.HP[1];
            System.Console.WriteLine(this.name+" has restored "+heal+" points of Health! Health: "+this.HP[0]+"/"+this.HP[1]);
        }
        internal void boostTimer() {    // Setting value for boost timers
            if (boost_timer[0] > 0)  boost_timer[0]--;
            else if (boost_value[0]!=0) {//defense stance
                boost_type[0]=false;
                DR -= boost_value[0];
                boost_value[0] = 0;
            }
            
            if (boost_timer[1]>0) boost_timer[1]--;
            else if (boost_value[1]!=0) {//adventuring boost
                boost_type[1]=false; 
                for(int i = 0; i < attr.Length; i++){
                    attr[i] -= boost_value[1];
                }
                boost_value[1] = 0;
            }

            if(boost_timer[2]>0)boost_timer[2]--;
            else if (boost_value[2]!=0) {//dexterity boost
                boost_type[2]=false;
                attr[1] -= boost_value[2];
                boost_value[2] = 0;
            }

            if(boost_timer[3]>0)boost_timer[3]--;
            else if (boost_value[3]!=0) {//evasion boost
                boost_type[3]=false;
                DR -= boost_value[3];
                boost_value[3] = 0;
            }

            if(boost_timer[4]>0)boost_timer[4]--;
            else if (boost_value[4]!=0) {//body boost
                boost_type[4]=false;
                for(int i = 0; i < 3; i++){
                    attr[i] -= boost_value[4];
                }
                boost_value[4] = 0;
            }

            if(boost_timer[5]>0)boost_timer[5]--;
            else if (boost_value[5]!=0) {//mind boost
                boost_type[5]=false;
                for(int i = 3; i < 6; i++){
                    attr[i] -= boost_value[5];
                }
                boost_value[5] = 0;
            }

            if(boost_timer[6]>0)boost_timer[6]--;
            else if (boost_value[6]!=0) {//holy boost
                boost_type[6]=false;
                for(int i = 0; i < this.attr.Length; i++){
                    attr[i] -= boost_value[6];
                }
                boost_value[6] = 0;
            }
        }

        internal void boost(string v) { // Activating boost based on string v
            switch (v){
                case "defense_stance":
                    if(!boost_type[0]){
                        this.boost_type[0] = true;
                        this.boost_timer[0] = Math.Max(this.attr[3]/3,1);
                        this.boost_value[0] = Math.Max(this.attr[3]/5,2);
                        this.DR += this.boost_value[0];
                    } else this.boost_timer[0] = Math.Max(this.attr[3]/3,1);
                    break;
                case "adventuring_boost":
                    if(!boost_type[1]){
                        this.boost_type[1] = true;
                        this.boost_timer[1] = Math.Max((this.attr[2]+this.attr[3])/5,1);
                        this.boost_value[1] = Program.roll(this.attr[6],1);
                        for(int i = 0; i < this.attr.Length; i++){
                            this.attr[i] += this.boost_value[1];
                        }
                        this.DR += this.boost_value[0];
                    } else this.boost_timer[1] = Math.Max(this.attr[3]/3,1); 
                    break;
                case "dexterity_boost":
                    if(!boost_type[2]){
                        this.boost_type[2]=true;
                        this.boost_timer[2] = Math.Max(this.attr[3]/4,1);
                        this.boost_value[2]=Program.roll(this.attr[6]*2,1)+Math.Max(this.attr[4]/2,5);
                        this.attr[1] += boost_value[2];
                    } else this.boost_timer[2] = Math.Max(this.attr[3]/3,1); 
                    break;
                case "evasion_boost":
                    if(!boost_type[3]){
                        this.boost_type[3]=true;
                        this.boost_timer[3] = Math.Max(this.attr[3]/5,1);
                        this.boost_value[3]=Program.roll(this.attr[6],1)+Math.Max(this.attr[4]/3,5);
                        this.DR += this.boost_value[3];
                     }else this.boost_timer[3] = Math.Max(this.attr[3]/5,1); 
                    break;
                case "body_boost":
                    if(!boost_type[4]){
                        this.boost_type[4] = true;
                        this.boost_timer[4] = Math.Max((this.attr[0]+this.attr[2]+this.attr[3])/10,1);
                        this.boost_value[4] = Program.roll(this.attr[6],1)+Math.Max(this.attr[3]/5,2);
                        for(int i = 0; i < 3; i++){
                            this.attr[i] += this.boost_value[4];
                        }
                    } else this.boost_timer[4] = Math.Max(this.attr[3]/3,1);
                    break;
                case "mind_boost":
                    if(!boost_type[5]){
                        this.boost_type[5]=true;
                        this.boost_timer[5] = Math.Max(this.attr[3]/6,1);
                        this.boost_value[5] = Program.roll(this.attr[6],1)+Math.Max(this.attr[5]/3,5);
                        for(int i = 3; i < 6; i++){
                            this.attr[i] += this.boost_value[5];
                        }
                    } else this.boost_timer[5] = Math.Max(this.attr[3]/6,1); 
                    break;
                case "holy_boost":
                    if(!boost_type[6]){
                        this.boost_type[6] = true;
                        this.boost_timer[6] = Math.Max(this.attr[6]/10,1);
                        this.boost_value[6] = Program.roll(this.attr[6],1)+Math.Max((this.attr[3]+this.attr[6])/20,1);
                        for(int i = 0; i < this.attr.Length; i++){
                            this.attr[i] += this.boost_value[6];
                        }
                        this.DR += this.boost_value[0];
                    } else this.boost_timer[4] = Math.Max(this.attr[3]/3,1);
                    break;
                case "all":
                    for (int i = 0; i < this.attr.Length; i++) {
                        this.attr[i]++;
                        this.setStats();
                    }
                    break;
                case "allMega":
                    for (int i = 0; i < this.attr.Length; i++) {
                        this.attr[i]+=10;
                        this.setStats();
                    }
                    break;
            }
        }
    }
}