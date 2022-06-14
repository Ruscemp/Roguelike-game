namespace Rougelike
{
    public class Dungeon
    {
        internal static float danger = 1;   // Danger value
        internal static int room = 0;   // Number of rooms entered
        string text = "";   // Text string for room descriptions
        roomType type;  // Stored enum for rool type
        Character PC;   // Storing Player Character
        Character? enemy;   // Storing enemy character
        bool done = false, temp_b;  // done bool for loops, and temp_b for temporary bools
        int? target = null; // Storing target value of challenges
        string? state = null;   // Storing current state of game via string
        string? temp_s; // temp_s for temporary strings
        int[] temp_i = new int[10]; // temp_i array for temporary integers

        internal Dungeon(){ // Starting text
            do{
                System.Console.WriteLine("Welcome to he Rouglike game!");
                System.Console.WriteLine("What do you want to do?");
                System.Console.WriteLine("1: Create a new Character | 2: Exit game");
                switch(System.Console.ReadLine()){
                    case "1": case "c": case "C": case "Create": case "Create a new Character":
                        done = true;
                        break;
                    case "2": case "e": case "E": case "Exit": case "Exit game":
                        Program.exit(0);
                        break;
                    default:
                        System.Console.Clear();
                        System.Console.WriteLine("Invalid input try again!\n");
                        break;
                }
            }while(!done);
            PC = Character.newChar();
        }

        internal void enterRoom() { // Select room type and display room text when entering the room
            state = null;
            System.Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");;
            text = "You enter a new room and look around. Danger level: "+(int)danger+"\n";
            switch(Program.roll(1,8,0)){
                case 0:
                    text += "The room is empty and has only one way forward.";
                    type = roomType.Empty;
                    danger += 0.1f;
                    break;
                case 1: case 2:
                    text += "The room contains an altar dedicated to unknown being.";
                    type = roomType.Altar;
                    danger += 0.5f;
                    break;
                case 3: case 4:
                    text += "The way forward is blocked by a puzzle.";
                    type = roomType.Puzzle;
                    danger += 2f;
                    break;
                case 5:
                    text += "The way forward is blocked by an enemy that immediately charges to attack you!";
                    type = roomType.Battle;
                    enemy = Character.newEnemy();
                    danger -= danger*0.25f;
                    break;
                case 6: case 7: case 8:
                    text += "The room contains a pool with mystical water.";
                    type = roomType.Pool;
                    danger += 1f;
                    break;
            }
            charPrint();
            System.Console.WriteLine(text);
        }

        internal void action()  // Display avaible actions and take players input to activate apropriate function
        {
            System.Console.WriteLine("What action do you take?");
            temp_s = "0: Exit the dungeon \n";
            switch (type) {
                case roomType.Empty:
                    temp_s +="1: Continue to next room";
                    break;
                case roomType.Altar:
                    temp_s +="1: Continue to next room \n2: Pray to the unknown being";
                    break;
                case roomType.Puzzle:
                    temp_s +="1: Try to solve the puzzle (character) \n2: Try to solve the puzzle (player)";
                    break;
                case roomType.Battle:
                    temp_s = "0: Flee and exit the dungeon \n1: Fight the enemy";
                    break;
                case roomType.Pool:
                    temp_s +="1: Continue to next room \n2: Drink from mystical pool\n3: Bath in mystical pool"; 
                    break;
            }
            System.Console.WriteLine(temp_s);
            switch(System.Console.ReadLine()){
                case "0":
                    state = "exit";
                    break;
                case "1":
                    switch(type){
                        case roomType.Empty:
                        case roomType.Altar:
                        case roomType.Pool:
                            state = "continue";
                            break;
                        case roomType.Puzzle:
                            state = "solve(auto)";
                            break;
                        case roomType.Battle:
                            state = "fight";
                            break;
                    }
                    break;
                case "2":
                    switch (type){
                        case roomType.Puzzle:
                            state = "solve(manual)";
                            break;
                        case roomType.Altar:
                            state = "pray";
                            break;
                        case roomType.Pool:
                            state = "drink";
                            break;
                    }
                    break;
                case "3":
                    switch (type){
                        case roomType.Pool:
                            state = "bath";
                            break;
                    }
                    break;
            }
        }

        internal void start() { // Main game function
            done = false;
            System.Console.WriteLine("Entering the dungeon!");
            enterRoom();
            do {   
                if(state == null) action();
                switch (state){
                    case "continue":
                        state = null;
                        enterRoom();
                        break;
                    case "solve(auto)":
                        state = null;
                        solve(true);
                        break;
                    case "solve(manual)":
                        state = null;
                        solve(false);
                        break;
                    case "fight":
                        state = null;
                        fight();
                        break;
                    case "pray":
                        state = null;
                        pray();
                        break;
                    case "drink":
                        state = null;
                        mystical(true);
                        break;
                    case "bath":
                        state = null;
                        mystical(false);
                        break;
                    case "exit":
                        Program.exit(PC.exitScore());
                        break;
                    default:
                        state = null;
                        System.Console.WriteLine("Wrong input!\nTry again!");
                        break;
                }
            } while (!done);
        }

        private void mystical(bool drinks) {    // When player drinks or baths in pool of mystical water
            if (drinks){
                switch(Program.roll(1,3)){
                    case 1:case 2:
                        System.Console.WriteLine("Mystical water heals you completly!");
                        PC.restoreHealth(PC.HP[1]);
                        break;
                    case 3:
                        System.Console.WriteLine("Mystical water does nothing to you!");
                        break;
                }
            }else{
                switch(Program.roll(1,9)){
                    case 1: case 2:case 3:case 4:case 5:
                        System.Console.WriteLine("Mystical water boosts you somewhat.");
                        PC.boost("all");
                        PC.boost("all");
                        PC.boost("all");
                        PC.restoreHealth(PC.HP[1]);
                        break;
                    case 6:case 7:
                        System.Console.WriteLine("Mystical water boosts you greatly.");
                        PC.boost("allMega");
                        PC.restoreHealth(PC.HP[1]);
                        break;
                    case 8:
                        System.Console.WriteLine("Mystical water Boosts you enormously");
                        PC.boost("allMega");
                        PC.boost("allMega");
                        PC.boost("allMega");
                        PC.restoreHealth(PC.HP[1]);
                        break;
                    case 9:
                        System.Console.WriteLine("Your body was not prepared for enormous amount of energy within mystical water and therefore was damaged even if it became somewhat strengthener also.");
                        PC.takeDamage(PC.HP[1]/4);
                        PC.boost("all");
                        break;
                }
            }
            state = "continue";
        }

        private void pray() // When player preys to unknown being with or without sacrificing blood
        {   
            target = (int)(danger*Program.roll(1,1000));
            pray:
            System.Console.WriteLine("\nDo you wish to sacrifice some of your blood to the unknown being alongside your prayer?");
            System.Console.WriteLine("y: Yes | n: No");
            switch (System.Console.ReadLine()){
                case "y": case "yes": case "Yes":
                    temp_b = true;
                    break;
                case "n": case "no": case "No":
                    temp_b = false;
                    break;
                default:
                    System.Console.WriteLine("Incorrect input\nTry again!");
                    goto pray;
            }
            if (temp_b){
                System.Console.WriteLine("How much of your blood do you sacrifice? Current health: "+PC.HP[0]+"/"+PC.HP[1]);
                temp_s = System.Console.ReadLine() ?? "0";
                if(int.TryParse(temp_s, out temp_i[0])){
                    if(temp_i[0] == 0){
                        System.Console.WriteLine("Your hesitate and change your mind about blood sacrifice.");
                    } else if (temp_i[0] < 0) {
                        System.Console.WriteLine("You try to cheat the unknown being by trying to draw blood from them!");
                        temp_i[0] = Program.roll(1,20);
                        if(temp_i[0]>15){
                            temp_i[0]-= 15;
                            System.Console.WriteLine("You even succeed in that!");
                            temp_i[0] = Program.roll(temp_i[0], Program.roll(temp_i[0], 2, 0));
                            PC.restoreHealth(temp_i[0]);
                            if(temp_i[0]==5){
                                System.Console.WriteLine("Even more incredible is that you managed to hide it from this unknown being!");
                            }else{
                                System.Console.WriteLine("Though this has angered the unknown being!");
                                target += Math.Max(temp_i[0]*25/10,1);
                            }
                        }
                    } else {
                        System.Console.WriteLine("You sacrifice "+temp_i[0]+" points of healt!");
                        PC.takeDamage(temp_i[0]);
                        System.Console.WriteLine(target);
                        target -= target*(Math.Max(temp_i[0]/50,1));
                        System.Console.WriteLine(target);
                    }
                }
            }
            temp_i[0] = Program.roll(Math.Max(PC.bonus(3)/5,1),(6+PC.bonus(6)));
            if(temp_i[0] >= target){
                temp_i[0] = Program.roll((1+PC.bonus(6)), (int)(1+(temp_i[0]-target)));
                temp_i[1] = Program.roll(1,6,0);
                PC.attr[temp_i[1]]+=temp_i[0];
                System.Console.WriteLine("The unknown being answers your prayer and grants you a boon increasing your "+Character.parseAttribute(temp_i[1].ToString())+ " by "+ temp_i[0]+" points!");
            }else{
                System.Console.WriteLine("Your prayer remains unanswred.");
            }
            state = "continue";
        }

        internal void solve(bool auto){ // When player solves puzzle questions manually or automatically ith rolls
            if (target == null) target = (int)danger*(Program.roll(10,20)/10);
            if(auto){
                int roll = Program.roll(3,6)+PC.bonus(4);
                if (roll >= target){
                    target = null;
                    System.Console.WriteLine("You solved the puzzle corretcly!");
                    PC.attr[Program.roll(1,6,4)]++;
                    state = "continue";
                }else{
                    System.Console.WriteLine("You triggered a trap! Fail margin: "+(target-roll));
                    PC.takeDamage(Program.roll(5,2,0));
                }
            }else{
                if (target < 50) System.Console.WriteLine("What begins with four, continues with two and ends with three?");
                else if (target < 100)  System.Console.WriteLine("What has no color, is around you, and you use it every day?");
                else if (target < 200)  System.Console.WriteLine("What flattens all mountains, wipes out all species, destroys every building, and turns everything into dust?");
                else System.Console.WriteLine("When you say my name, I'm no longer there. What am I?");
                bool correct;
                switch((System.Console.ReadLine()??"").ToLower()){
                    case "human":
                        if(target < 50 ){
                            correct = true;
                            break;
                        }else{
                            goto default;
                        }
                    case "air":
                        if(target >= 50 && target < 100){
                            correct = true;
                            break;
                        }else{
                            goto default;
                        }
                    case "time":
                        if(target >= 100 && target < 200){
                            correct = true;
                            break;
                        }else{
                            goto default;
                        }
                    case "silence":
                        if(target >= 200){
                            correct = true;
                            break;
                        }else{
                            goto default;
                        }
                    default:
                        correct = false;
                        break;
                }
                if (correct){
                    state = "continue";
                    target = null;
                    System.Console.WriteLine("You solved the puzzle corretcly!");
                    PC.attr[Program.roll(1,6,4)]+=2;
                }else{
                    System.Console.WriteLine("You triggered a trap! Incorrect answer");
                    PC.takeDamage(Program.roll(5,2,0));
                }

            }
        }

        internal void fight() { //When player begins a fight with enemy|Display appropiate text, take input and use appropriate functions
            System.Console.WriteLine("Beginning the FIGHT!");
            if (enemy == null) enemy = Character.newEnemy();
            while (enemy.HP[0]>0){
                PC.boostTimer();
                charPrint();
                System.Console.WriteLine("Your action?");
                temp_s = "0: Flee the Dungeon\n1: Base attack\n2: Defense stance";
                if (PC.boost_type[0]) temp_s+=" (cooldown: "+PC.boost_timer[0]+")";
                temp_s+="\n3: ";
                switch(PC.role) {
                    case charRole.Adventurer:
                        temp_s += "Adventuring boost!";
                        if(PC.boost_type[1]) temp_s += " (cooldown: "+PC.boost_timer[1]+")";
                        break;
                    case charRole.Thief:
                        temp_s += "Dexterity boost!";
                        if(PC.boost_type[2]) temp_s +=  "(cooldown: "+PC.boost_timer[2]+")";
                        break;
                    case charRole.Archer:
                        temp_s += "Evasion boost!";
                        if(PC.boost_type[3]) temp_s +=  "(cooldown: "+PC.boost_timer[3]+")";
                        break;
                    case charRole.Warrior:
                        temp_s += "Body boost!";
                        if(PC.boost_type[4]) temp_s +=  "(cooldown: "+PC.boost_timer[4]+")";
                        break;
                    case charRole.Wizard:
                        temp_s += "Mind boost!";
                        if(PC.boost_type[5]) temp_s +=  "(cooldown: "+PC.boost_timer[5]+")";
                        break;
                    case charRole.Priest:
                        temp_s += "Holy boost!";
                        if(PC.boost_type[6]) temp_s +=  "(cooldown: "+PC.boost_timer[6]+")";
                        break;
                }
                System.Console.WriteLine(temp_s);
                switch (System.Console.ReadLine()) {
                    case "0":
                        Program.exit(PC.exitScore());
                        break;
                    case "1":
                        PC.baseAttack(enemy);
                        break;
                    case "2":
                        PC.boost("defense_stance");
                        break;
                    case "3":
                        switch(PC.role){
                            case charRole.Adventurer:
                                PC.boost("adventuring_boost");
                                break;
                            case charRole.Thief:
                                PC.boost("dexterity_boost");
                                break;
                            case charRole.Archer:
                                PC.boost("evasion_boost");
                                break;
                            case charRole.Warrior:
                                PC.boost("body_boost");
                                break;
                            case charRole.Wizard:
                                PC.boost("mind_boost");
                                break;
                            case charRole.Priest:
                                PC.boost("holy_boost");
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if(enemy.HP[0]>0){
                    switch(Program.roll(1,9)){
                        case 1: case 2: case 3: case 4: case 5: case 6:
                            //System.Console.WriteLine(enemy.name+" attacks!");
                            enemy.baseAttack(PC);
                            break;
                        case 7: case 8:
                            danger+=0.5f;
                            System.Console.WriteLine(enemy.name+" boosts!");
                            enemy.boost("all");
                            break;
                        case 9:
                            temp_i[1] = Math.Max((enemy.attr[3]),1);
                            System.Console.WriteLine(enemy.name+" heals!");
                            enemy.restoreHealth(temp_i[1]);
                            break;
                    }
                }
            };
            enemy = null;
            state = "continue";
        }

        private void charPrint() {  // Display character information
            String s1="", s2="", s3="", s4="", s5="", s6="";
            s1 = String.Format("{0,-4} {1,-10} {2,-5} {3,-5}","Name:",PC.name,"Role:",PC.role);
            s2 = String.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-3} {5,-7}","DR:",PC.DR,"Luck:",PC.attr[6],"HP:", PC.HP[0]+"/"+PC.HP[1]);
            s3 = String.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-5} {5,-5}","Str:",PC.attr[0],"Dex:",PC.attr[1],"End:",PC.attr[2]);
            s4 = String.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-5} {5,-5}","Will:",PC.attr[3],"Int:",PC.attr[4],"Wis:",PC.attr[5]);
            s5 = String.Format("{0,25} {1,-9}","\u2193 Active \u2193 Boosts \u2193","");
            if (enemy != null){
                s3 += String.Format("{0,0}","||||");
                s4 += String.Format("{0,-8} {1,-5} {2,-10}","|VS|","Name:",enemy.name);
                s5 += String.Format("{0,-3} {1,-3} {2,-7} {3,-3} {4,-7}","||||","HP:",enemy.HP[0]+"/"+enemy.HP[1],"DR: ",enemy.DR);
            }
            if(PC.boost_type[0]) s6 += String.Format("{0,-15}","Defense("+PC.boost_value[0]+":"+PC.boost_timer[0]+")");
            if(PC.boost_type[1]) s6 += String.Format("{0,-15}","Adventuring("+PC.boost_value[1]+":"+PC.boost_timer[1]+")");
            if(PC.boost_type[2]) s6 += String.Format("{0,-15}","Dexterity("+PC.boost_value[2]+":"+PC.boost_timer[2]+")");
            if(PC.boost_type[3]) s6 += String.Format("{0,-15}","Evasion("+PC.boost_value[3]+":"+PC.boost_timer[3]+")");
            if(PC.boost_type[4]) s6 += String.Format("{0,-15}","Body("+PC.boost_value[4]+":"+PC.boost_timer[4]+")");
            if(PC.boost_type[5]) s6 += String.Format("{0,-15}","Mind("+PC.boost_value[5]+":"+PC.boost_timer[5]+")");
            if(PC.boost_type[6]) s6 += String.Format("{0,-15}","Holy("+PC.boost_value[6]+":"+PC.boost_timer[6]+")");

            
            System.Console.WriteLine(s1);
            System.Console.WriteLine(s2);
            System.Console.WriteLine(s3);
            System.Console.WriteLine(s4);
            System.Console.WriteLine(s5);
            System.Console.WriteLine(s6);
        }
    }
}