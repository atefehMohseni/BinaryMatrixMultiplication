using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Matrix
{
    class Program
        {
            static void Main(string[] args)
            {
                var mtr = new int[8, 8];
                Console.WriteLine("Enter (8*8) Binay Matrix \n");
                for (var i = 0; i < 8; i++)
                {
                    var line = Console.ReadLine();
                    var spl = line.Split(' ');
                    if (spl.Length != 8) throw new FormatException();
                    for (var j = 0; j < 8; j++)
                        mtr[i, j] = int.Parse(spl[j]);
                }
                int[] x = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                //*****
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 256; j++)
                        answertable.T[i, j] = 1;
                //********
                //do these phases for all possible xi combination
                var watch = Stopwatch.StartNew();
                // the code that you want to measure comes here


                for (int xi = 0; xi < 256; xi++)
                {
                   // xi =7;
                    Console.WriteLine(xi);
                    x = convertX2binary(x);
                    if (xi != 0)
                        x = decimal2Binary(xi);

                    int[,] multi = new int[8, 8];
                    multi = multipleMatrix(mtr, x);

                    package[] groups = new package[8];

                    //phase1: create packages
                    groups = package.createPackages(multi);

                    //phase2: check any possible answer of the package 
                    int mustCalculateDirectly = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (!groups[i].emptypackage)                            
                            mustCalculateDirectly = package.findPossibleAnswerofPackages(groups, i, multi);
                        

                        if (mustCalculateDirectly == -1)
                        {
                            //shows any possible error
                        }
                    }

                    //phase3: merg the answers 
                    int[] firstAnswer = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };

                    int packagecounter = 0;
                    for (int c = 0; c < 8; c++)
                       if (!groups[c].emptypackage)
                          packagecounter++;

                    groups= package.sortemptyPackages(groups);
                   // package.MergPossibleAnswerOfPackages(firstAnswer, multi,0, packagecounter, groups, xi);
                    package.MergAnswers(packagecounter-1, groups, xi);
                

                    //phase4: save record in T[i,j]

                }//end loops for xi!


                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                string row = "  ";
                StringBuilder sb = new StringBuilder(257);
                StreamWriter sw = new StreamWriter(".\\report.txt");


                Console.WriteLine();

                for (int i = 0; i < 256; i++)
                {
                    row = "";
                    Console.Write(i + "' ");
                    row += i + "' ";
                    for (int j = 0; j < 256; j++)
                    {
                        Console.Write(answertable.T[i, j] + " ");
                        row += answertable.T[i, j] + " ";
                    }
                    Console.WriteLine();
                    row += Environment.NewLine;
                    sb.Append(row);
                   

                }

                Console.WriteLine(elapsedMs + "msec");
                //insert row number
               int t = 7;
                for (int i = 0; i < 256; i++)
                {
                    if(answertable.T[t,i]==0)
                      Console.WriteLine(i + answertable.T[t, i]);
                }
                sw.Write(sb.ToString());
             

            }
            /// <summary>
            /// this function convert the value of X array to binary
            /// </summary>
            /// <param name="X"> each one is  byte</param>
            /// <returns>1 if was not equal to zero</returns>
            static int[] convertX2binary(int[] X)
            {
                int[] x = new int[8];

                for (int i = 0; i < 8; i++)
                {
                    if (X[i] != 0)
                        x[i] = 1;
                    else
                        x[i] = 0;
                }
                return x;
            }

            static int[] decimal2Binary(int decimalnum)
            {
                int[] binary = new int[8];

                for (int i = 0; i < 8; i++)
                    binary[i] = 0;

                int j = 7;
                while (decimalnum != 1)
                {
                    binary[j] = decimalnum % 2;
                    decimalnum = decimalnum / 2;
                    j--;
                }
                binary[j] = 1;
                return binary;
            }
            /// <summary>
            /// zarbe m dar x
            /// </summary>
            /// <param name="M">M 8*8 binary matrix</param>
            /// <param name="x">x 8*1 binary matrix</param>
            /// <returns>hasel e.x (x1+x2[0,0,0,1,1])</returns>
            static int[,] multipleMatrix(int[,] M, int[] x)
            {
                //each row contain an array to know each yi have xi e.x y1=[0,0,0,0,1,0,1,0] ~ y1=x2+x4
                int[,] result = new int[8, 8];

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                        result[i, j] = M[i, j] * x[j];
                }

                return result;
            }

        }

        public class answertable
        {

            public static int[,] T = new int[256, 256];
            answertable()
            {
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 256; j++)
                        T[i, j] = 1;
            }
        }

        /// <summary>
        /// packegs include yi which are related to each other(have same index of xi)
        /// </summary>
        /// 
        public class package
        {
            package()
            {
                parent = -1;
                child = new int[8];
                sameParent = new int[8];

                for (int i = 0; i < 8; i++)
                {
                    child[i] = 0;
                    sameParent[i] = 0;//sameparent[4]=1; means y4 is same with this->parent
                }
                answerCount = 0;
              //  emptypackage = false;
            }

            /// <summary>
            /// the index of yi (the learder of the packeg!)
            /// </summary>
            public int parent;

            /// <summary>
            /// int[8] child; child[4]=1 this means y4 is the child of this packeg(parent) or not(~child[4]=0)
            /// </summary>
            int[] child;

            /// <summary>
            /// represent the index of yj s in matrix which are same with parent of this package
            /// </summary>
            int[] sameParent;
            
            /// <summary>
            ///represent the repeatness of this package in prevoius package(saved in sameParent) 
            /// </summary>
           public  bool emptypackage;
            /// <summary>
            ///refers to yi which are present in this packeg (the some of parent and child)
            /// </summary>
            int packegeMember;

            /// <summary>
            /// number of correct states that the package's(group) member can be created. 
            /// </summary>
            int answerCount;

            /// <summary>
            /// matrix (2^(packagemember-1) * 8) contains possible answer of the package(which include "packagemember" yi)
            /// </summary>
            int[,] possibleAnswer;

            //  static package[] groups= new package[8];

            /// <summary>
            /// return the number Of NonZero Xi Indexs in Y(e.x:for y1=x1+x3+x7 is 3)
            /// </summary>
            /// <param name="y"></param>
            /// <param name="i"></param>
            /// <returns></returns>
            static int numberOfNonZeroXiIndexsInY(int[,] y, int i)
            {
                int numberOfNonZeroXiIndexsInY = 0;

                for (int j = 0; j < 8; j++)
                    if (y[i, j] != 0)
                        numberOfNonZeroXiIndexsInY++;

                return numberOfNonZeroXiIndexsInY;
            }

            /// <summary>
            /// return the number Of same Xi Indexs in Yi s(e.x:for y1=x1+x3+x7 and y2=x5+x3+x7 is 2)
            /// </summary>
            /// <param name="y"></param>
            /// <param name="i">index of our yi</param>
            /// <param name="j">index of previous yi(want to compare)</param>
            /// <returns></returns>
            static int numberOfSameXiIndexs(int[,] y, int i, int j)
            {
                int numberOfSameXiIndexs = 0;

                for (int k = 0; k < 8; k++)
                    if (y[i, k] != 0 && y[i, k] == y[j, k])
                        numberOfSameXiIndexs++;

                return numberOfSameXiIndexs;
            }

            /// <summary>
            /// for grouping yi's they must be sorted
            /// </summary>
            /// <param name="y"></param>
            /// <returns>y'[0]=3 : means y3 has minimum lenght in yi's</returns>
            static int[] sortYascending(int[,] y)
            {
                int[] yprim = new int[8];
                int[] sortedY = new int[8];

                int min = 100;//just a big number
                int indexMin = 0;

                for (int i = 0; i < 8; i++)
                    yprim[i] = numberOfNonZeroXiIndexsInY(y, i);

                //sort
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (yprim[j] < min)
                        {
                            min = yprim[j];
                            indexMin = j;
                        }
                    }
                    sortedY[i] = indexMin;
                    yprim[indexMin] = 100;
                    min = 100;
                }
                return sortedY;
            }

            /// <summary>
            /// extract group of packeges(contains one or more yi) from y
            /// which their xi have dependancy to eachother
            /// </summary>
            /// <param name="y"></param>
            /// <returns></returns>
            public static package[] createPackages(int[,] y)
            {
                package[] groups = new package[8];
                for (int i = 0; i < 8; i++)
                    groups[i] = new package();

                int[] sortedY = new int[8];
                sortedY = sortYascending(y);
                
                int diff=0;//difference between parents(yi)
                bool newpackage = true;

                for (int i = 0; i < 8; i++)
                {
                    //does yi saved before in diff package(for repeated yi in matrix)
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (!groups[j].emptypackage)
                        {
                            diff = numberOfNonZeroXiIndexsInY(y, sortedY[i]) - numberOfNonZeroXiIndexsInY(y, groups[j].parent);
                            if (diff == 0 && numberOfSameXiIndexs(y, sortedY[i], groups[j].parent) == numberOfNonZeroXiIndexsInY(y, groups[j].parent))
                            {
                                //dont record yi in new package coz it is same with previous
                                groups[j].sameParent[sortedY[i]] = 1;

                                groups[i].emptypackage = true;
                                newpackage = false;//this is not a new package
                            }
                        }
                    }
                    if (newpackage)
                    {
                        groups[i].emptypackage = false;//also it was false by defualt
                        groups[i].parent = sortedY[i];//refers to index of sorted yi(index started from zero)
                        groups[i].packegeMember++;

                        // check feasability of previous packeges to be here (in this packeg)
                        if (numberOfNonZeroXiIndexsInY(y, sortedY[i]) >= 3)//the minimum lenght of each child is 2(so the parent should be 3 or more)
                        {
                            //check previous packeges
                            for (int j = i - 1; j >= 0; j--)
                            {
                                //at least must have 2 same xi index for merging!(the groups)
                                //e.x: yj=x1+x2 and yi=x1+x2+x3
                                if(!groups[j].emptypackage)
                                if (numberOfSameXiIndexs(y, sortedY[i], groups[j].parent) >= 2)
                                {
                                    //and also greater lenght for PARENT is necessary
                                    if (numberOfNonZeroXiIndexsInY(y, sortedY[i]) >= numberOfNonZeroXiIndexsInY(y, groups[j].parent) + 1)
                                    {
                                        //move the packege of yj to be a child of yi
                                        groups[i].child[groups[j].parent] = 1;
                                       // groups[i].child[j] = 1;

                                        groups[i].packegeMember++;
                                        //  break;
                                    }

                                }
                            }
                        }
                    }//add new package(and if)
                    newpackage = true;
                }

                return groups;
            }

           
            public static void MergAnswers(int packageindex, package[] groups,int xi)
            {
                //to merg answers
                int[] chunckanswer = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                bool yiIsknown = true;
                int yj = -1;

                int p1 , p2 , p3 , p4 , p5 ,p6 , p7 , p8 ;
                p1 = p2 = p3 = p4 = p5 = p6 = p7 = p8 = 0;

                if(packageindex>=0)
                    for (int r = 0; r < groups[packageindex].answerCount; r++)
                    {
                        p1 = packageindex;
                        for (int c = 0; c < 8; c++)
                            if (groups[packageindex].possibleAnswer[r, c] != -1)
                                chunckanswer[c] = groups[packageindex].possibleAnswer[r, c];
                        //test chun answr compeletness

                        for (int i = 0; i < 8; i++)
                            if (chunckanswer[i] == -1)
                            {
                                yiIsknown = false;//there is at least one unknown yi
                                break;
                            }
                        if (yiIsknown)
                        {
                            yj = binary2decimal(chunckanswer);
                            answertable.T[xi, yj] = 0;
                        }
                        if (!yiIsknown)
                        {
                            yiIsknown = true;
                            packageindex--;
                            if (packageindex >= 0)
                            {
                                p2 = packageindex;
                                for (int r2 = 0; r2 < groups[packageindex].answerCount; r2++)
                                      {
                                        for (int c = 0; c < 8; c++)
                                            if (groups[packageindex].possibleAnswer[r2, c] != -1 && chunckanswer[c] == -1)
                                                chunckanswer[c] = groups[packageindex].possibleAnswer[r2, c];
                                        //test chun answr compeletness

                                        for (int i = 0; i < 8; i++)
                                            if (chunckanswer[i] == -1)
                                            {
                                                yiIsknown = false;//there is at least one unknown yi
                                                break;
                                            }
                                        if (yiIsknown)
                                        {
                                            yj = binary2decimal(chunckanswer);
                                            answertable.T[xi, yj] = 0;
                                        }
                                        if (!yiIsknown)
                                        {
                                            yiIsknown = true;
                                            packageindex--;
                                            if (packageindex >= 0)
                                            {
                                                p3 = packageindex;
                                                //**********************************
                                                //******************* 3************
                                                /////////////////////////////////////
                                                for (int r3 = 0; r3 < groups[packageindex].answerCount; r3++)
                                                {
                                                    for (int c = 0; c < 8; c++)
                                                        if (groups[packageindex].possibleAnswer[r3, c] != -1 && chunckanswer[c] == -1)
                                                            chunckanswer[c] = groups[packageindex].possibleAnswer[r3, c];
                                                    //test chun answr compeletness

                                                    for (int i = 0; i < 8; i++)
                                                        if (chunckanswer[i] == -1)
                                                        {
                                                            yiIsknown = false;//there is at least one unknown yi
                                                            break;
                                                        }
                                                    if (yiIsknown)
                                                    {
                                                        yj = binary2decimal(chunckanswer);
                                                        answertable.T[xi, yj] = 0;
                                                    }
                                                    if (!yiIsknown)
                                                    {
                                                        yiIsknown = true;
                                                        packageindex--;
                                                        if (packageindex >= 0)
                                                        {
                                                            //*****************************************
                                                            /////////////////  4
                                                            ////////////////////////////////////////////
                                                            p4 = packageindex;
                                                            for (int r4 = 0; r4 < groups[packageindex].answerCount; r4++)
                                                            {
                                                                for (int c = 0; c < 8; c++)
                                                                    if (groups[packageindex].possibleAnswer[r4, c] != -1 && chunckanswer[c] == -1)
                                                                        chunckanswer[c] = groups[packageindex].possibleAnswer[r4, c];
                                                                //test chun answr compeletness

                                                                for (int i = 0; i < 8; i++)
                                                                    if (chunckanswer[i] == -1)
                                                                    {
                                                                        yiIsknown = false;//there is at least one unknown yi
                                                                        break;
                                                                    }
                                                                if (yiIsknown)
                                                                {
                                                                    yj = binary2decimal(chunckanswer);
                                                                    answertable.T[xi, yj] = 0;
                                                                }
                                                                if (!yiIsknown)
                                                                {
                                                                    yiIsknown = true;
                                                                    packageindex--;
                                                                    if (packageindex >= 0)
                                                                    {
                                                                        p5 = packageindex;
                                                                        //**********************************
                                                                        ///          5
                                                                        ////////////////////////////////////
                                                                        for (int r5 = 0; r5 < groups[packageindex].answerCount; r5++)
                                                                        {
                                                                            for (int c = 0; c < 8; c++)
                                                                                if (groups[packageindex].possibleAnswer[r5, c] != -1 && chunckanswer[c] == -1)
                                                                                    chunckanswer[c] = groups[packageindex].possibleAnswer[r5, c];
                                                                            //test chun answr compeletness

                                                                            for (int i = 0; i < 8; i++)
                                                                                if (chunckanswer[i] == -1)
                                                                                {
                                                                                    yiIsknown = false;//there is at least one unknown yi
                                                                                    break;
                                                                                }
                                                                            if (yiIsknown)
                                                                            {
                                                                                yj = binary2decimal(chunckanswer);
                                                                                answertable.T[xi, yj] = 0;
                                                                            }
                                                                            if (!yiIsknown)
                                                                            {
                                                                                yiIsknown = true;
                                                                                packageindex--;
                                                                                if (packageindex >= 0)
                                                                                {
                                                                                    ////////////////////////////////
                                                                                    //////        6
                                                                                    ////////////////////////////////
                                                                                    p6 = packageindex;
                                                                                    for (int r6 = 0; r6 < groups[packageindex].answerCount; r6++)
                                                                                    {
                                                                                        for (int c = 0; c < 8; c++)
                                                                                            if (groups[packageindex].possibleAnswer[r6, c] != -1 && chunckanswer[c] == -1)
                                                                                                chunckanswer[c] = groups[packageindex].possibleAnswer[r6, c];
                                                                                        //test chun answr compeletness

                                                                                        for (int i = 0; i < 8; i++)
                                                                                            if (chunckanswer[i] == -1)
                                                                                            {
                                                                                                yiIsknown = false;//there is at least one unknown yi
                                                                                                break;
                                                                                            }
                                                                                        if (yiIsknown)
                                                                                        {
                                                                                            yj = binary2decimal(chunckanswer);
                                                                                            answertable.T[xi, yj] = 0;
                                                                                        }
                                                                                        if (!yiIsknown)
                                                                                        {
                                                                                            yiIsknown = true;
                                                                                            packageindex--;
                                                                                            if (packageindex >= 0)
                                                                                            {
                                                                                                //////////////////////////////////////////
                                                                                                ///////////           7
                                                                                                //////////////////////////////////////////
                                                                                                p7 = packageindex;
                                                                                                for (int r7 = 0; r7 < groups[packageindex].answerCount; r7++)
                                                                                                {
                                                                                                    for (int c = 0; c < 8; c++)
                                                                                                        if (groups[packageindex].possibleAnswer[r7, c] != -1 && chunckanswer[c] == -1)
                                                                                                            chunckanswer[c] = groups[packageindex].possibleAnswer[r7, c];
                                                                                                    //test chun answr compeletness

                                                                                                    for (int i = 0; i < 8; i++)
                                                                                                        if (chunckanswer[i] == -1)
                                                                                                        {
                                                                                                            yiIsknown = false;//there is at least one unknown yi
                                                                                                            break;
                                                                                                        }
                                                                                                    if (yiIsknown)
                                                                                                    {
                                                                                                        yj = binary2decimal(chunckanswer);
                                                                                                        answertable.T[xi, yj] = 0;
                                                                                                    }
                                                                                                    if (!yiIsknown)
                                                                                                    {
                                                                                                        yiIsknown = true;
                                                                                                        packageindex--;
                                                                                                        if (packageindex >= 0)
                                                                                                        {
                                                                                                            /////////////////////////////////////////
                                                                                                            ////////////    8
                                                                                                            ////////////////////////////////////////
                                                                                                            for (int r8 = 0; r8 < groups[packageindex].answerCount; r8++)
                                                                                                            {
                                                                                                                for (int c = 0; c < 8; c++)
                                                                                                                    if (groups[packageindex].possibleAnswer[r8, c] != -1 && chunckanswer[c] == -1)
                                                                                                                        chunckanswer[c] = groups[packageindex].possibleAnswer[r8, c];
                                                                                                                //test chun answr compeletness

                                                                                                                for (int i = 0; i < 8; i++)
                                                                                                                    if (chunckanswer[i] == -1)
                                                                                                                    {
                                                                                                                        yiIsknown = false;//there is at least one unknown yi
                                                                                                                        break;
                                                                                                                    }
                                                                                                                if (yiIsknown)
                                                                                                                {
                                                                                                                    yj = binary2decimal(chunckanswer);
                                                                                                                    answertable.T[xi, yj] = 0;
                                                                                                                }
                                                                                                                if (!yiIsknown)
                                                                                                                {
                                                                                                                    yiIsknown = true;
                                                                                                                    packageindex--;
                                                                                                                    if (packageindex >= 0)
                                                                                                                    {

                                                                                                                    }//end if had seconf package
                                                                                                                }//end if there is third package
                                                                                                            }//end surfing on second package
                                                                                                        }//end if had seconf package
                                                                                                    }//end if there is third package
                                                                                                    packageindex = p7;//dont needed
                                                                                                }//end surfing on second package
                                                                                            }//end if had seconf package
                                                                                        }//end if there is third package
                                                                                        packageindex = p6;
                                                                                    }//end surfing on second package
                                                                                }//end if had seconf package
                                                                            }//end if there is third package
                                                                            packageindex = p5;
                                                                        }//end surfing on second package
                                                                    }//end if had seconf package
                                                                }//end if there is third package
                                                                packageindex = p4;
                                                            }//end surfing on second package
                                                        }//end if had seconf package
                                                    }//end if there is third package
                                                    packageindex = p3;
                                                }//end surfing on third package
                                            }//end if had seconf package
                                        }//end if there is third package
                                        packageindex = p2;
                                }//end surfing on second package
                           
                     
                            }//end if had second package
                        }//end continue searching after first checking yj(first parent package)
                        packageindex = p1;
                    }//end for serching on big group
                
            }

            public static package[] sortemptyPackages(package[] groups)
            {
                package[] sortedgroups = new package[8];
                int packagecount=0;
                packagecount = 0;

                for(int i=0;i<8;i++)
                    if(!groups[i].emptypackage)
                    {
                        sortedgroups[packagecount] = groups[i];
                        packagecount++;
                    }
                return sortedgroups;
            }
            
            public static int binary2decimal(int[] binary)
            {
                int result = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (binary[i] != 0)
                        result += Convert.ToInt16(Math.Pow(2, (7-i)));//7-i
                }
                return result;
            }

           
            /// <summary>
            /// extract the possible answer of a package while contains several child(other yi)
            /// </summary>
            /// <param name="Parentknownxi">present known xi indexs of parent which was declared</param>
            /// <param name="answerchunk">present yi indexs of parent(when will be completed then save to possibleanswer of parent)</param>
            /// <param name="childindex">present index of a child which must merg its possible answer to answerchunk </param>
            /// <param name="parentindex">present index of the parent</param>
            /// <param name="influencedchild">present parent's important child index</param>
            /// <param name="groups"></param>
            /// <param name="y"></param>
            public static int extractPossibleAnswer(int[] Parentknownxi, int[] answerchunk, int childindex, int parentindex, int[] influencedchild, package[] groups, int[,] y,int rowindexOfLastPossibleAnswer)
            {
                if (childindex != -1)//if no child found for parent then this func called with childindex -1
                {
                    int[] myanswerChunk = new int[8];
                    // myanswerChunk = answerChunk;
                    for (int i = 0; i < 8; i++)
                        myanswerChunk[i] = answerchunk[i];

                    //fetch each possible answer of each children then merg to othr
                    
                    for (int i = 0; !groups[childindex].emptypackage && i < groups[childindex].answerCount; i++)
                    {
                        if (answerchunk[groups[childindex].parent] == -1)//this child answer was not considered before(y1=x1+x2+x3 y2=x1+x2  so dontt need to check y2 again in this example)
                        {
                            for (int j = 0; j < 8; j++)//yj
                                if (groups[childindex].possibleAnswer[i, j] != -1)
                                {
                                    answerchunk[j] = groups[childindex].possibleAnswer[i, j];//copy all yj answers ,childs know!

                                    //update Parentknownxi
                                    for (int k = 0; k < 8; k++)//xk
                                    {
                                        if (y[j, k] == 1)//retrive xk of yj
                                            Parentknownxi[k] = 1;
                                    }
                                }
                            //to check wether the chunck answer has been compeleted or not
                            bool tocontinue = false;
                            for (int j = 0; j < 3; j++)//check childrens
                            {
                                if (influencedchild[j] != -1)//this child exist in the package
                                {
                                    if (!groups[influencedchild[j]].emptypackage)
                                    if (answerchunk[groups[influencedchild[j]].parent] == -1)
                                        tocontinue = true;
                                }
                                else
                                    break;
                            }

                            if (tocontinue)
                            {
                                //call this function for next child
                                int k;
                                for (k = 0; k < 3; k++)
                                    if (influencedchild[k] == childindex)
                                        break;
                                //k is our index between infulenced children
                                //call next child(k+1)
                                if (k < 2 && influencedchild[k + 1] != -1)
                                {
                                   rowindexOfLastPossibleAnswer=extractPossibleAnswer(Parentknownxi, answerchunk, influencedchild[k + 1], parentindex, influencedchild, groups, y, rowindexOfLastPossibleAnswer);
                                }

                                tocontinue = false;
                            }
                            else//(!tocontinue)
                            {
                                //1.check does our children covered all parent xi indexes -> then just XOR them to know yparent
                                //2.otherwise we have to consider possible answer for remain xis in yparent
                                bool childcoverness = true;
                                int[] remainxi = new int[8];//initial to 0
                                int numberofremainedxi = 0;

                                for (int k = 0; k < 8; k++)
                                    if(!groups[parentindex].emptypackage)
                                    if ( y[groups[parentindex].parent, k]==1 && Parentknownxi[k] != y[groups[parentindex].parent, k])
                                    {
                                        childcoverness = false;
                                        remainxi[k] = 1;
                                        numberofremainedxi++;
                                    }

                                //retrive the value of our childs (yj)
                                int[] indexofelements = new int[] { -1, -1, -1 };
                                int[] elements = new int[] { -1, -1, -1 };
                                int numberofelements = 0;

                                for (int j = 0; j < 3; j++)
                                    if (influencedchild[j] != -1)
                                    {
                                        if (!groups[influencedchild[j]].emptypackage)
                                        indexofelements[j] =groups[influencedchild[j]].parent;//the yj index of children
                                        elements[j] = answerchunk[groups[influencedchild[j]].parent];
                                        numberofelements++;
                                    }


                                //1.
                                if (childcoverness)//our child totally covered parent
                                {
                                    //the parent result is simpley XOR their children
                                    int[] parentresult = new int[]{ 0,0};
                                    parentresult = XOR(elements,indexofelements,numberofelements,y);

                                    //add this answerchunk to parent possible answers
                                    if (parentresult[0] == 1)
                                    {
                                        if(!groups[parentindex].emptypackage)
                                        answerchunk[groups[parentindex].parent] = 0;

                                        for (int c = 0; c < 8;c++ )
                                            if (groups[parentindex].sameParent[c]==1)
                                                answerchunk[c] = 0;


                                            for (int l = 0; l < 8; l++)
                                                groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                        rowindexOfLastPossibleAnswer++;
                                        groups[parentindex].answerCount++;
                                    }
                                    if (parentresult[1] == 1)
                                    {
                                        answerchunk[groups[parentindex].parent] = 1;
                                        for (int c = 0; c < 8; c++)
                                            if (groups[parentindex].sameParent[c] == 1)
                                                answerchunk[c] = 1;

                                        for (int l = 0; l < 8; l++)
                                            groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                        rowindexOfLastPossibleAnswer++;
                                        groups[parentindex].answerCount++;
                                    }
                                }//end if totally coverness

                                  //2. some xi indexes is not assigned yet(by childrens)
                                else
                                {
                                    int[] elementsresult = new int[]{0,0};
                                    elementsresult = XOR(elements,indexofelements,numberofelements,y);

                                    //extract remain xi
                                    if (numberofremainedxi == 1)
                                    {
                                        //this xi can not get the value equal to zero(single xi is always 1)
                                        if (elementsresult[1] == 1)
                                        {
                                            answerchunk[groups[parentindex].parent] = 1;//1

                                            for (int c = 0; c < 8;c++ )
                                                if(groups[parentindex].sameParent[c] == 1)
                                                    answerchunk[c] = 1;

                                                for (int l = 0; l < 8; l++)
                                                    groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                            rowindexOfLastPossibleAnswer++;
                                            groups[parentindex].answerCount++;
                                           
                                            //***********
                                            answerchunk[groups[parentindex].parent] = 0;
                                            for (int c = 0; c < 8; c++)
                                                if (groups[parentindex].sameParent[c] == 1)
                                                    answerchunk[c] = 0;

                                            for (int l = 0; l < 8; l++)
                                                groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                            rowindexOfLastPossibleAnswer++;
                                            groups[parentindex].answerCount++;
                                        }
                                        if (elementsresult[0] == 1)
                                        {
                                            answerchunk[groups[parentindex].parent] = 1;//1
                                            for (int c = 0; c < 8; c++)
                                                if (groups[parentindex].sameParent[c] == 1)
                                                    answerchunk[c] = 1;


                                            for (int l = 0; l < 8; l++)
                                                groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                            rowindexOfLastPossibleAnswer++;
                                            groups[parentindex].answerCount++;
                                        }
                                        // if parent result was zero there is no option for remain xi and this is not a possible answer for parent
                                    }
                                    else//more than one xi exist
                                    {
                                        //there r two possible answer for remain xi(0-1)
                                        //so the result for parent is both(0 and 1)

                                        //result will be 0
                                        answerchunk[groups[parentindex].parent] = 0;
                                        for (int c = 0; c < 8; c++)
                                            if (groups[parentindex].sameParent[c] == 1)
                                                answerchunk[c] = 0;

                                        for (int l = 0; l < 8; l++)
                                            groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                        rowindexOfLastPossibleAnswer++;
                                        //result will be 1
                                        answerchunk[groups[parentindex].parent] = 1;
                                        for (int c = 0; c < 8; c++)
                                            if (groups[parentindex].sameParent[c] == 1)
                                                answerchunk[c] = 1;

                                        for (int l = 0; l < 8; l++)
                                            groups[parentindex].possibleAnswer[rowindexOfLastPossibleAnswer, l] = answerchunk[l];

                                        rowindexOfLastPossibleAnswer++;
                                        groups[parentindex].answerCount++;
                                    }

                                }//end else halatgiri
                                //this function called for compeleting parent answers and also return change of rowindex
                            }//end else not to continue calling function
                        }
                        // myanswerChunk = answerChunk;
                        for (int c = 0; c < 8; c++)
                           answerchunk[c]= myanswerChunk[c];
                    }//end for surfing on child possible answer
                    if(!groups[parentindex].emptypackage)
                    rowindexOfLastPossibleAnswer = remainChildOfthePackagePossibleAnswers(groups, parentindex, rowindexOfLastPossibleAnswer, y);

                }//end if there is a child

                else
                {
                    //the package have no any child!
                    if (!groups[parentindex].emptypackage)
                    {
                        groups[parentindex].answerCount = 2;
                        groups[parentindex].possibleAnswer = new int[2, 8];
                        for (int k = 0; k < 8; k++)
                        {
                            groups[parentindex].possibleAnswer[0, k] = -1;
                            groups[parentindex].possibleAnswer[1, k] = -1;
                        }
                        groups[parentindex].possibleAnswer[0, groups[parentindex].parent] = 0;
                        groups[parentindex].possibleAnswer[1, groups[parentindex].parent] = 1;

                      //  groups[parentindex].answerCount += 2;

                        for(int j=0;j<8;j++)
                            if (groups[parentindex].sameParent[j] != 0)
                            {
                                groups[parentindex].possibleAnswer[0, j] = 0;
                                groups[parentindex].possibleAnswer[1, j] = 1;
                            }
                    }
                }
                return rowindexOfLastPossibleAnswer;
            }//end function

            /// <summary>
            /// compute the XOR of elements
            /// </summary>
            /// <param name="indexofelements">represent the yj indexes of children</param>
            /// <param name="numofelement"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static int[] XOR(int[] elementsvalue, int[] indexofelements,int numofelement,int[,]y)
            {
                int[] result =new int[]{0,0};//result[0]=1 means the result could be 0
                int countrerBit1=0;
                bool overlap = false;

                switch (numofelement)
                {
                    case 1:
                        //no overlap could be exist
                        break;
                    case 2:
                        if(numberOfSameXiIndexs(y,indexofelements[0],indexofelements[1]) >= 1)
                            overlap=true;
                        break;
                    case 3:
                        if (numberOfSameXiIndexs(y, indexofelements[0], indexofelements[1]) >= 1 || numberOfSameXiIndexs(y, indexofelements[0], indexofelements[2]) >= 1 || numberOfSameXiIndexs(y, indexofelements[1], indexofelements[2]) >= 1)
                            overlap = true;
                        break;
                }
                for (int i = 0; i < 3; i++)
                    if (elementsvalue[i] == 1)
                        countrerBit1++;

                if (countrerBit1 % 2 == 0)
                {
                    result[0] = 1;

                    if (overlap)
                        result[1] = 1;

                }
                else
                {
                    result[1]=1;

                    if (overlap)
                        result[0] = 1;
                }

                return result;
            }

            
            /// <summary>
            /// return the possible value for x where was Xor with element(we know the result and want to fetch possible answer for remain element)
            /// </summary>
            /// <param name="elements">index of the child's brother</param>
            /// <param name="numberofelements"></param>
            /// <param name="resultindex"></param>
            /// <returns></returns>
            public static int[] inverseXOR(int[] indexOfelements, int numberofelements, int parentindex, int resultindex, package[] groups,int[,]y,int myindex)
            {

                int[] cangetvalue = new int[] { 0, 0 };//first col refers to 0 and the second refers to 1
                int countrBit1 = 0;

                int[] myelements = new int[8];//my childrens value
                int c = 0;
                int broindex = 0;

                int dadpossibleanswer=-1;

                if (!groups[parentindex].emptypackage)
                {
                    dadpossibleanswer = groups[parentindex].possibleAnswer[resultindex, groups[parentindex].parent];

                    for (int i = 0; i < 8; i++)
                    {
                        if (indexOfelements[i] != -1)
                        {
                            if (!groups[indexOfelements[i]].emptypackage)
                                broindex = groups[indexOfelements[i]].parent;
                            if (groups[parentindex].possibleAnswer[resultindex, broindex] != -1)//this bro was set
                            {
                                myelements[c] = groups[parentindex].possibleAnswer[resultindex, broindex];
                                c++;
                            }
                        }
                    }
                    //first XOR the elements           
                    for (int i = 0; i < numberofelements; i++)
                        if (myelements[i] == 1)
                            countrBit1++;

                    int xorbrothers = 0;//the result of xor bigbrothers
                    if (countrBit1 % 2 != 0)
                        xorbrothers = 1;

                    //to check overlap between brothers
                    bool overlap = false;
                    if (!groups[myindex].emptypackage)
                        switch (numberofelements)
                        {
                            case 1:
                                if (numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[0]) >= 1)
                                    overlap = true;
                                break;
                            case 2:
                                if (numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[0]) >= 1 || numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[1]) >= 1)
                                    overlap = true;
                                break;
                            case 3:
                                if (numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[0]) >= 1 || numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[1]) >= 1 || numberOfSameXiIndexs(y, groups[myindex].parent, indexOfelements[2]) >= 1)
                                    overlap = true;

                                break;
                        }

                    if (dadpossibleanswer == 1)
                    {
                        if (xorbrothers == 0)
                        {
                            cangetvalue[1] = 1;//just can get the value equal to 1

                            if (overlap)
                                cangetvalue[0] = 1;
                        }
                        else
                        {
                            cangetvalue[0] = 1;
                            //zero is for some cases that dad have odd 1 bit(totaly)
                             if (overlap)
                               cangetvalue[1] = 1;
                        }
                    }
                    else//parentindex=0
                    {
                        if (xorbrothers == 1)
                        {
                            cangetvalue[1] = 1;

                            //  if (overlap)
                            //    cangetvalue[0] = 1;
                        }
                        else
                        {
                            cangetvalue[0] = 1;
                            //  if (overlap)
                            //    cangetvalue[1] = 1;
                        }
                    }
                }
                return cangetvalue;
            }

           
            /// <summary>
            /// return the index of yi in y to calculate y possible answer more efficient (by  xi answers)
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="packagetIndex">the index of parent(means yi)</param>
            /// <returns>an integer array contains zero or one(if indexOfimportantChild[j]=1 means yi will use yj answers) returns package index of yj</returns>
            public static int[] indexOfimportantChild(int[,] y, package[] groups, int packagetIndex)
            {
                                
                int[] indexOfimportantChild = new int[3];
                int parentLenght = numberOfNonZeroXiIndexsInY(y, groups[packagetIndex].parent);
                int parentIndex = groups[packagetIndex].parent;

                //we will have maximum 3 groups(child)
                int i = -1;
                int j = -1;
                int k = -1;

                int fitness = 0;//fitness means more similarity
                int bestfitness = -1;

                for (int c1 = packagetIndex - 1; c1 >= 0; c1--)
                {
                    if(!groups[c1].emptypackage)
                    if (groups[packagetIndex].child[groups[c1].parent] == 1)//we have this child(and also its lenght is biger than 2(cause groups rule!))
                    {
                        fitness = numberOfSameXiIndexs(y, parentIndex, groups[c1].parent);

                        for (int c2 = c1 - 1; c2 >= 0; c2--)
                        {
                            //important children r whom have no conflict to each other
                            //y1=x1+x2+x3 and y2=x1+x2 are not a pair of important children!
                            if(!groups[c2].emptypackage)
                            if (groups[c1].child[groups[c2].parent] == 0 &&//second children of parent (not to be child of first choosen child)
                                groups[packagetIndex].child[groups[c2].parent] == 1 &&
                                numberOfNonZeroXiIndexsInY(y, groups[c2].parent) < numberOfNonZeroXiIndexsInY(y, groups[c1].parent)
                              )
                            {
                                fitness += numberOfSameXiIndexs(y, parentIndex, groups[c2].parent) - numberOfSameXiIndexs(y, groups[c1].parent, groups[c2].parent);
                                //check out can we have 3 important child?
                                if (parentLenght - fitness >= 2)
                                {
                                    for (int c3 = c2 - 1; c3 >= 0; c3--)
                                    {
                                        if (!groups[c3].emptypackage)
                                        if (groups[c1].child[groups[c3].parent] == 0 &&//second children of parent (not to be child of first choosen child)
                                            groups[packagetIndex].child[groups[c3].parent] == 1 &&
                                            numberOfNonZeroXiIndexsInY(y, groups[c3].parent) < numberOfNonZeroXiIndexsInY(y, groups[c1].parent)
                                           )
                                            fitness += numberOfSameXiIndexs(y, parentIndex, groups[c3].parent) - (numberOfSameXiIndexs(y, groups[c2].parent, groups[c3].parent) + numberOfSameXiIndexs(y, groups[c1].parent, groups[c3].parent));

                                        if (fitness > bestfitness)
                                        {
                                            bestfitness = fitness;
                                            i = c1;
                                            j = c2;
                                            k = c3;
                                            //do not add other fitness(while r in c2 loop(crossing over c3 possible children))
                                            fitness -= numberOfSameXiIndexs(y, parentIndex, groups[c3].parent) - (numberOfSameXiIndexs(y, groups[c2].parent, groups[c3].parent) + numberOfSameXiIndexs(y, groups[c1].parent, groups[c3].parent));
                                        }
                                    }
                                }
                                else
                                {
                                    //just have 2 subchild(refresh the best fitness)
                                    if (fitness > bestfitness)
                                    {
                                        bestfitness = fitness;
                                        i = c1;
                                        j = c2;
                                        //do not add other fitness(while r in c1 loop(crossing over c2 possible children))
                                        fitness -= numberOfSameXiIndexs(y, parentIndex, groups[c2].parent) - numberOfSameXiIndexs(y, groups[c1].parent, groups[c2].parent);
                                    }
                                }
                            }
                        }
                        if (fitness > bestfitness)
                        {
                            bestfitness = fitness;
                            i = c1;
                            //do not add other fitness(while r in c2 loop(crossing over c3 possible children))
                            fitness = 0;
                        }
                    }//end finding one sub child
                }
                                
                //i, j, k r package index
                
                // ? check : are childrens totally covered parent? reurn them
                
                // dont return thoes children had overlap to each other
                if(i != -1 && j!=-1)
                if (numberOfSameXiIndexs(y, groups[i].parent, groups[j].parent) >= 1)
                    j = -1;//dont return j


                indexOfimportantChild[0] = i;
                indexOfimportantChild[1] = j;
                indexOfimportantChild[2] = k;

                return indexOfimportantChild;
            }

            /// <summary>
            /// this function finds each possible answer of packages
            /// e.x: y1=x1+x2 have 2 answer 1 or 0
            /// e.x:y1=x1+x2+x3 can use y1 answers too
            /// </summary>
            /// <param name="groups"></param>
            public static int findPossibleAnswerofPackages(package[] groups, int indexOftheparentOfPackage, int[,] y)
            {
                int negativeIndexToShowNoResultFound = -1;

                //if yi is zero(contains no xi)(so the yi in any other possible answers must be fixed 0)
                if (numberOfNonZeroXiIndexsInY(y, groups[indexOftheparentOfPackage].parent) == 0)
                {
                    groups[indexOftheparentOfPackage].possibleAnswer = new int[1, 8];
                    for (int c = 0; c < 8; c++)
                        groups[indexOftheparentOfPackage].possibleAnswer[0, c] = -1;
                    
                    int indexofyi = groups[indexOftheparentOfPackage].parent;
                    groups[indexOftheparentOfPackage].possibleAnswer[0, indexofyi] = 0;
                    //check for other same parent 
                    for (int i = 0; i < 8;i++ )
                        if(groups[indexOftheparentOfPackage].sameParent[i] != 0)
                            groups[indexOftheparentOfPackage].possibleAnswer[0, i] = 0;

                        groups[indexOftheparentOfPackage].answerCount = 1;

                    return indexOftheparentOfPackage;
                }

                //yi just have one xi, (so the yi in any possible answer must be 1)
                else if (numberOfNonZeroXiIndexsInY(y, groups[indexOftheparentOfPackage].parent) == 1)
                {
                    groups[indexOftheparentOfPackage].possibleAnswer = new int[1, 8];
                    for (int c = 0; c < 8; c++)
                        groups[indexOftheparentOfPackage].possibleAnswer[0, c] = -1;

                    int indexofyi = groups[indexOftheparentOfPackage].parent;
                    groups[indexOftheparentOfPackage].possibleAnswer[0, indexofyi] = 1;
                   
                    //check for other same parent 
                    for (int i = 0; i < 8; i++)
                        if (groups[indexOftheparentOfPackage].sameParent[i] != 0)
                            groups[indexOftheparentOfPackage].possibleAnswer[0, i] = 1;

                    groups[indexOftheparentOfPackage].answerCount = 1;

                    return indexOftheparentOfPackage;
                }

                //yi has no child and contains of two xi (there is two possible answer zero or one)
                else if (numberOfNonZeroXiIndexsInY(y, groups[indexOftheparentOfPackage].parent) == 2)
                {
                    groups[indexOftheparentOfPackage].possibleAnswer = new int[2, 8];

                    for (int c = 0; c < 8; c++)
                    {
                        groups[indexOftheparentOfPackage].possibleAnswer[0, c] = -1;
                        groups[indexOftheparentOfPackage].possibleAnswer[1, c] = -1;
                    }
                    int indexofyi = groups[indexOftheparentOfPackage].parent;
                    
                        groups[indexOftheparentOfPackage].possibleAnswer[1, indexofyi] = 1;
                        groups[indexOftheparentOfPackage].answerCount++;
                    
                        groups[indexOftheparentOfPackage].possibleAnswer[0, indexofyi] = 0;
                        groups[indexOftheparentOfPackage].answerCount++;

                        //check for other same parent 
                        for (int i = 0; i < 8; i++)
                            if (groups[indexOftheparentOfPackage].sameParent[i] != 0)
                            {
                                groups[indexOftheparentOfPackage].possibleAnswer[1, i] = 1;
                                groups[indexOftheparentOfPackage].possibleAnswer[0, i] = 0;
                            }

                    return indexOftheparentOfPackage;
                }

                //yi maybe has a child which computed before
                else
                {
                    int maxRowsOfOurPossibleAnswer = 0;
                    double oursxi = numberOfNonZeroXiIndexsInY(y, groups[indexOftheparentOfPackage].parent);
                    maxRowsOfOurPossibleAnswer = Convert.ToInt16(Math.Pow(2, oursxi + 1));//? need to be more space
                    groups[indexOftheparentOfPackage].possibleAnswer = new int[maxRowsOfOurPossibleAnswer, 8];

                    for (int r = 0; r < maxRowsOfOurPossibleAnswer; r++)
                        for (int c = 0; c < 8; c++)
                            groups[indexOftheparentOfPackage].possibleAnswer[r, c] = -1;
                    //
                    //call indexof important child
                    //
                    int[] influencedchild = new int[] { -1, -1, -1 };
                   
                    influencedchild=indexOfimportantChild(y, groups, indexOftheparentOfPackage);

                    int havechild = 0;

                    if (influencedchild[0] == -1)
                        havechild = -1;
                    else
                        havechild = influencedchild[0];

                    int[] parentfirstindex=new int[]{0,0,0,0,0,0,0,0};
                    int[] answerchunk=new int[]{-1,-1,-1,-1,-1,-1,-1,-1};

                    if (!groups[indexOftheparentOfPackage].emptypackage)
                    extractPossibleAnswer(parentfirstindex, answerchunk, havechild, indexOftheparentOfPackage, influencedchild, groups, y, 0);
                    
                    //*******************
                    //to check if any yi has no value yet
                    //
                    //checkremainyiinthepackage

                    return 1;
                    //*********

                }
                //do nothing!
                return negativeIndexToShowNoResultFound;//for answering must analyze yi directly


            }//end find possible func 


            public static int remainChildOfthePackagePossibleAnswers(package[] groups, int i, int indexOfLastRowpossibleAnswerOfY,int[,]y)
            {
                //test does any yi remain?
                // bool remainyi = false;
                int[] bigbrotherindexs = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };//(to be or not to be my brother)if bigbrothers[2]=1 : y2 is my bigbrother which its value has been set before
                int[] sortedRemainchild = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };//max can be 5(for the gurd keep 8)

                int rowCount = 0;
                int numberofremainyi = 0;
               // rowCount = groups[i].answerCount;
                rowCount = indexOfLastRowpossibleAnswerOfY;
                
                if (rowCount <= 0)
                {
                    //throw error
                    //there is no possible answer in table and he wants to fetch remain children!!
                    return indexOfLastRowpossibleAnswerOfY;
                }
                    
                //int b = indexOfLastRowpossibleAnswerOfY;

                for (int h = i - 1; h >= 0; h--)//to have sotred remain yi nozuli
                    if(!groups[h].emptypackage)
                    if (groups[i].child[groups[h].parent] == 1)
                    {
                        if (groups[i].possibleAnswer[rowCount-1,groups[h].parent] == -1)
                        {
                            sortedRemainchild[numberofremainyi] = groups[h].parent;//the index of remain yi
                            numberofremainyi++;
                        }
                        else if (groups[i].possibleAnswer[rowCount - 1, groups[h].parent] != -1)
                            bigbrotherindexs[groups[h].parent] = 1;
                    }

                int children = -1;

                //  int[] dadpossibleanswers = new int[] { 0, 0 };//does dad can get the value equal to zero(dadpossible[0]) wt about 1(dadpossible[1]=1 means yes he can)

                int dadpossibleanswer = -1;

                int[] mybrothersindex = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                int[] myxorresult = new int[] { 0, 0 };
                int counter = 0;
                bool usedrow = false;
               
                //************* added here
                /*
                for (int j = 0; j < 8; j++)
                    if (bigbrotherindexs[j] == 1)
                    {
                        mybrothersindex[counter] = j;// groups[i].possibleAnswer[k, j];
                        counter++;
                    }
                 * */
                //*************************************************

                for (int k = 0; k < rowCount && groups[i].possibleAnswer[k, groups[i].parent] != -1; k++)
                {
                    for (int c = 0; c < numberofremainyi; c++)
                    {
                        //retrive remain yi
                        children = sortedRemainchild[c];

                        //xor bigbrothers first

                        dadpossibleanswer = groups[i].possibleAnswer[k, groups[i].parent];

                        //didnt move above!
                        for (int j = 0; j < 8; j++)
                            if (bigbrotherindexs[j] == 1)
                            {
                                mybrothersindex[counter] = j;// groups[i].possibleAnswer[k, j];
                                counter++;
                            }
                        
                        myxorresult = inverseXOR(mybrothersindex, counter, i, k, groups, y, sortedRemainchild[c]); 
                        //update possible answer row
                        if (myxorresult[0] == 1)
                        {
                            groups[i].possibleAnswer[k, sortedRemainchild[c]] = 0;

                            // groups[i].answerCount++;
                            usedrow = true;
                        }
                        if (myxorresult[1] == 1)
                        {
                            if (!usedrow)
                            {
                                groups[i].possibleAnswer[k, sortedRemainchild[c]] = 1;

                                // groups[i].answerCount++;
                            }
                            else
                            {
                                //copy the possible answer row (because for this possible row there is two other possible answer)
                                for (int l = 0; l < 8; l++)
                                    groups[i].possibleAnswer[indexOfLastRowpossibleAnswerOfY, l] = groups[i].possibleAnswer[k, l];

                                groups[i].possibleAnswer[indexOfLastRowpossibleAnswerOfY, sortedRemainchild[c]] = 1;

                               
                                indexOfLastRowpossibleAnswerOfY++;
                                groups[i].answerCount++;
                            }
                        }

                        //update big brother
                        // added here
                        //***************************
                        for (int h = i - 1; h >= 0; h--)//to have sotred remain yi nozuli
                            if (!groups[h].emptypackage)
                                if (groups[i].child[groups[h].parent] == 1)
                                {
                                    if (groups[i].possibleAnswer[rowCount - 1, groups[h].parent] != -1)
                                        bigbrotherindexs[groups[h].parent] = 1;
                                }
                        counter = 0;
                        //***************************
                    }
                    //update remain yi(move detected yi to bigbrothers)
                    usedrow = false;
                    counter = 0;

                    //check assigned yj and copy these answers to any other sam eindex

                    for (int t = 0; t < 8; t++)
                    {
                        if (groups[i].possibleAnswer[k, t] != -1)//is assigned
                        {
                            //serach for same yi with t
                            for (int j = 0; j < 8;j++ )
                            {
                                if (groups[j].parent == t)
                                {
                                    //does have any same parent
                                    for(int n=0;n<8;n++)
                                        if (groups[j].sameParent[n] != 0)
                                        {//copy answer to k
                                            groups[i].possibleAnswer[k, n] = groups[i].possibleAnswer[k, t];
                                        }
                                }//end if(find yj package index)
                            }
                        }
                    }
                }//end for!
                int indexoflastrow = indexOfLastRowpossibleAnswerOfY;
                return indexoflastrow;
            }


        };

}
