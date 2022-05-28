using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace TestProb1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Attack
        {
            get { return (int)GetValue(AttackProperty); }
            set { SetValue(AttackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Attack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttackProperty =
            DependencyProperty.Register("Attack", typeof(int), typeof(MainWindow), new PropertyMetadata(4));

        public int Defence
        {
            get { return (int)GetValue(DefenceProperty); }
            set { SetValue(DefenceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Defence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefenceProperty =
            DependencyProperty.Register("Defence", typeof(int), typeof(MainWindow), new PropertyMetadata(2));

        public double ToHit
        {
            get { return (double)GetValue(ToHitProperty); }
            set { SetValue(ToHitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToHit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToHitProperty =
            DependencyProperty.Register("ToHit", typeof(double), typeof(MainWindow), new PropertyMetadata(30d));

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public double ToDef
        {
            get { return (double)GetValue(ToDefProperty); }
            set { SetValue(ToDefProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToDef.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToDefProperty =
            DependencyProperty.Register("ToDef", typeof(double), typeof(MainWindow), new PropertyMetadata(30d));

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public int DefendingFigures
        {
            get { return (int)GetValue(DefendingFiguresProperty); }
            set { SetValue(DefendingFiguresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefendingFigures.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefendingFiguresProperty =
            DependencyProperty.Register("DefendingFigures", typeof(int), typeof(MainWindow), new PropertyMetadata(2));

        public int AttackingFigures
        {
            get { return (int)GetValue(AttackingFiguresProperty); }
            set { SetValue(AttackingFiguresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttackingFigures.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttackingFiguresProperty =
            DependencyProperty.Register("AttackingFigures", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public int OneDefendingFigHP
        {
            get { return (int)GetValue(OneDefendingFigHPProperty); }
            set { SetValue(OneDefendingFigHPProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OneDefendingFigHP.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneDefendingFigHPProperty =
            DependencyProperty.Register("OneDefendingFigHP", typeof(int), typeof(MainWindow), new PropertyMetadata(2));

        public int RemaningHP
        {
            get { return (int)GetValue(RemaningHPProperty); }
            set { SetValue(RemaningHPProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemaningHP.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemaningHPProperty =
            DependencyProperty.Register("RemaningHP", typeof(int), typeof(MainWindow), new PropertyMetadata(4));

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void EfficientCalc()
        {
            //if (this.hitsRB.IsChecked == true)
            //{
            //    double toHit = this.ToHit / 100;
            //    double sum = 0;
            //    var result = new StringBuilder();
            //    for (int i = 0; i <= Attack; ++i)
            //    {
            //        var hitProb = BinomialResult(i, toHit, Attack);
            //        sum += hitProb;
            //        result.Append("P(Hits = ");
            //        result.Append(i);
            //        result.Append(") = ");
            //        result.AppendLine(String.Format("{0:0.00}", hitProb));
            //    }
            //    result.AppendLine("sum: " + String.Format("{0:0.00}", sum));
            //    this.Result = result.ToString();
            //}

            //if (this.damageRB.IsChecked == true)
            //{
            double toHit = this.ToHit / 100;
            double toDef = this.ToDef / 100;
            double sum = 0;
            var result = new StringBuilder();
            for (int damage = 0; damage <= Attack; ++damage)
            {
                double damageProb = 0d;
                var result2 = new StringBuilder();
                for (int hit = damage; hit <= Attack; ++hit)
                {
                    var probHit = BinomialResult(hit, toHit, Attack);
                    var defence = hit - damage;
                    //defence = defence < 0 ? 0 : defence;
                    //if (defence >= 0)
                    //{
                    if (damage == 0)
                    {
                        //defence = hit;
                        while (defence >= 0 && defence <= Defence)
                        {
                            //result2.Append($",({hit},{defence})");
                            var probDefence = BinomialResult(defence, toDef, Defence);
                            damageProb += probHit * probDefence;
                            ++defence;
                        }
                    }
                    else
                    {
                        if (defence >= 0 && defence <= Defence)
                        {
                            //result2.Append($",({hit},{defence})");
                            var probDefence = BinomialResult(defence, toDef, Defence);
                            damageProb += probHit * probDefence;
                        }
                    }
                    //}
                }
                sum += damageProb;
                result.Append("P(Damage = ");
                result.Append(damage);
                result.Append(") = ");
                result.Append(result2.ToString());
                result.AppendLine(String.Format("{0:0.00}", damageProb));
            }
            result.AppendLine("sum: " + String.Format("{0:0.00}", sum));
            this.Result = result.ToString();
            //}
        }

        void DefenceSteps(
            double[] dmgDist,
            double probReachThisStep,
            int dmgDoneSoFar,
            int attackHitsLeft,
            int defendingFigures,
            int healthPerDefFigure,
            int topDefendingFigureHP)
        {
            double toHit = this.ToHit / 100;
            double toDef = this.ToDef / 100;

            //if (Defence <= 0)
            //{
            //    dmgDoneSoFar = dmgDoneSoFar >= dmgDist.Length ? dmgDist.Length - 1 : dmgDoneSoFar;
            //    dmgDist[dmgDoneSoFar] += probReachThisStep;
            //    return;
            //}

            for (int def = 0; def <= Defence; ++def)
            {
                var probDefence = BinomialResult(def, toDef, Defence);
                var dmgDoneToFigure = attackHitsLeft - def;
                dmgDoneToFigure = dmgDoneToFigure < 0 ? 0 : dmgDoneToFigure;
                dmgDoneToFigure = dmgDoneToFigure > topDefendingFigureHP ? topDefendingFigureHP : dmgDoneToFigure;
                var attHitsLeft = attackHitsLeft - dmgDoneToFigure - def;
                attHitsLeft = attHitsLeft < 0 ? 0 : attHitsLeft;

                if (dmgDoneToFigure == topDefendingFigureHP) // killed the top defending figure
                {
                    var remaningFigures = defendingFigures - 1;
                    if (remaningFigures <= 0)
                    {
                        // no defending figures left, finish defending steps and update probability of this event sequence
                        var dmgInd = dmgDoneSoFar + dmgDoneToFigure;
                        dmgInd = dmgInd >= dmgDist.Length ? dmgDist.Length - 1 : dmgInd;
                        dmgDist[dmgInd] += probReachThisStep * probDefence;
                        continue;
                    }
                    else
                    {
                        if (attHitsLeft == 0)
                        {
                            // no attacker hits left , finish defending steps and update probability of this event sequence
                            var dmgInd = dmgDoneSoFar + dmgDoneToFigure;
                            dmgInd = dmgInd >= dmgDist.Length ? dmgDist.Length - 1 : dmgInd;
                            dmgDist[dmgInd] += probReachThisStep * probDefence;
                            continue;
                        }
                        else
                        {
                            Debug.Assert(attHitsLeft > 0);
                            DefenceSteps(
                                dmgDist,
                                probReachThisStep * probDefence,
                                dmgDoneSoFar + dmgDoneToFigure,
                                attHitsLeft,
                                remaningFigures,
                                healthPerDefFigure,
                                healthPerDefFigure);
                            continue;
                        }
                    }
                }
                else
                {
                    Debug.Assert(dmgDoneToFigure < topDefendingFigureHP && dmgDoneToFigure >= 0);
                    // did not kill top figure, stop defence steps and update probability of this event sequence

                    var dmgInd = dmgDoneSoFar + dmgDoneToFigure;
                    dmgInd = dmgInd >= dmgDist.Length ? dmgDist.Length - 1 : dmgInd;
                    dmgDist[dmgInd] += probReachThisStep * probDefence;
                    continue;
                }
            }
        }

        /// <summary>
        /// Updates dmgDist with a possible attack execution step
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="probReachThisStep"></param>
        /// <param name="defendingFigures"></param>
        /// <param name="healthPerDefFigure"></param>
        /// <param name="topDefendingFigureHP"></param>
        private void CalcAttackStep(
            double[] dmgDist,
            double probReachThisStep,
            int dmgDoneSoFar,
            int defendingFigures,
            int healthPerDefFigure,
            int topDefendingFigureHP)
        {
            if (Attack <= 0)
            {
                dmgDoneSoFar = dmgDoneSoFar >= dmgDist.Length ? dmgDist.Length - 1 : dmgDoneSoFar;
                dmgDist[dmgDoneSoFar] += probReachThisStep;
                return;
            }

            double toHit = this.ToHit / 100;
            double toDef = this.ToDef / 100;
            for (int hit = 0; hit <= Attack; ++hit)
            {
                var probHit = BinomialResult(hit, toHit, Attack);
                if (hit == 0)
                {
                    var dmgInd = dmgDoneSoFar;
                    dmgInd = dmgInd >= dmgDist.Length ? dmgDist.Length - 1 : dmgInd;
                    dmgDist[dmgInd] += probReachThisStep * probHit;
                    continue;
                }
                else
                {
                    DefenceSteps(
                        dmgDist,
                        probReachThisStep * probHit,
                        dmgDoneSoFar,
                        hit,
                        defendingFigures,
                        healthPerDefFigure,
                        topDefendingFigureHP);
                }
            }
        }

        private double[] GetZeroInitializedDamageDist(int remaningHP)
        {
            var dmgArr = new double[remaningHP + 1];
            for (var k = 0; k < dmgArr.Length; ++k)
            {
                dmgArr[k] = 0d;
            }
            return dmgArr;
        }

        private void GetRemaningFiguresHP(
            int healthPerDefFigure,
            int remaningHP,
            out int defendingFiguresOut,
            out int topDefendingFigureHPOut)
        {
            Debug.Assert(healthPerDefFigure > 0);
            Debug.Assert(remaningHP >= 0);

            topDefendingFigureHPOut = remaningHP % healthPerDefFigure;
            topDefendingFigureHPOut = topDefendingFigureHPOut == 0 ? healthPerDefFigure : topDefendingFigureHPOut;
            defendingFiguresOut = ((remaningHP - topDefendingFigureHPOut) / healthPerDefFigure)+1;
            Debug.Assert(remaningHP
                == ((defendingFiguresOut - 1) * healthPerDefFigure + topDefendingFigureHPOut));
            Debug.Assert(defendingFiguresOut >= 0);
            Debug.Assert(topDefendingFigureHPOut >= 0);
        }

        private int GetRemaningHP(
            int defendingFigures,
            int healthPerDefFigure,
            int topDefendingFigureHP)
        {
            var result = (defendingFigures - 1) * healthPerDefFigure + topDefendingFigureHP;
            Debug.Assert(result >= 0);
            return result;
        }

        private void CalcRemaining(int defendingFigures,
                            int healthPerDefFigure,
                            int topDefendingFigureHP,
                            int dmgDone,
                            out int defendingFiguresOut,
                            out int topDefendingFigureHPOut)
        {
            Debug.Assert(defendingFigures > 0);
            Debug.Assert(healthPerDefFigure > 0);
            Debug.Assert(topDefendingFigureHP > 0);
            Debug.Assert(dmgDone >= 0);
            var remainingDmg = GetRemaningHP(defendingFigures, healthPerDefFigure, topDefendingFigureHP) - dmgDone;
            topDefendingFigureHPOut = remainingDmg % healthPerDefFigure;
            topDefendingFigureHPOut = topDefendingFigureHPOut == 0 ? healthPerDefFigure : topDefendingFigureHPOut;
            defendingFiguresOut = ((remainingDmg - topDefendingFigureHPOut) / healthPerDefFigure)+1;
            Debug.Assert(
                ((defendingFigures - 1) * healthPerDefFigure + topDefendingFigureHP)
                == ((defendingFiguresOut - 1) * healthPerDefFigure + topDefendingFigureHPOut + dmgDone));
            Debug.Assert(defendingFiguresOut >= 0);
            Debug.Assert(topDefendingFigureHPOut >= 0);
        }

        /// <summary>
        /// returns the damage distribution of the given number of figures
        /// </summary>
        double[] CalcFiguresDamageDist(
            int attackingFigures,
            int defendingFigures,
            int healthPerDefFigure,
            int topDefendingFigureHP)
        {
            // lookup from damage done, to probablity of damage by a single attacking figure
            var remaningHPToDmgDist = new Dictionary<int, double[]>();

            //// the result to return
            //var resultDmg = GetZeroInitializedDamageDist(RemaningHP);

            // the updated damage probalbility distribution of previous figures
            // we initialize it to zero and update it by summing over all possible events
            var prevDmgArr = GetZeroInitializedDamageDist(RemaningHP);
            prevDmgArr[0] = 1d; // initially, before the first figure attacked, probability of zero damage is 100%

            for (int i = 1; i <= attackingFigures; ++i)
            {
                // probability of each damage after figure i has finished attacking/defense iteration
                // updated value for next step 'prevDmgArr'
                var updatedDmg = GetZeroInitializedDamageDist(RemaningHP);
                for (int previousDamage = 0; previousDamage < prevDmgArr.Length; ++previousDamage)
                {
                    var previousDmgProb = prevDmgArr[previousDamage];
                    if (previousDmgProb > 0)
                    {

                        var remaningHP = GetRemaningHP(defendingFigures,
                            healthPerDefFigure,
                            topDefendingFigureHP) - previousDamage;

                        // calculate current figure damage distribution (given previous damage) or find it in lookup
                        // (if it was already calculated) for performance considerations
                        var currFigureDamageDist = GetZeroInitializedDamageDist(RemaningHP);
                        if (true
                            //!remaningHPToDmgDist.TryGetValue(remaningHP, out currFigureDamageDist)
                            )
                        {
                            // if we can't find it in the lookup, calculate it and update the lookup
                            currFigureDamageDist = GetZeroInitializedDamageDist(RemaningHP);

                            // get updated stepDefendingFigures, steptopDefendingFigureHP
                            int stepDefendingFigures;
                            int steptopDefendingFigureHP;
                            CalcRemaining(defendingFigures,
                                                healthPerDefFigure,
                                                topDefendingFigureHP,
                                                previousDamage,
                                                out stepDefendingFigures,
                                                out steptopDefendingFigureHP);

                            // calculate currFigureDamageDist
                            CalcAttackStep(
                                currFigureDamageDist,
                                1d,
                                0,
                                stepDefendingFigures,
                                healthPerDefFigure,
                                steptopDefendingFigureHP);

                            // update the lookup for future searches
                            remaningHPToDmgDist[remaningHP] = currFigureDamageDist;
                        }

                        for (int currentFigureDmg = 0; currentFigureDmg < currFigureDamageDist.Length; ++currentFigureDmg)
                        {
                            var currFigureDmgProb = currFigureDamageDist[currentFigureDmg];
                            var dmgInd = previousDamage + currentFigureDmg;
                            dmgInd = dmgInd >= updatedDmg.Length ? updatedDmg.Length - 1 : dmgInd;
                            updatedDmg[dmgInd] += currFigureDmgProb * previousDmgProb;
                        }
                    }
                }

                prevDmgArr = updatedDmg;
            }

            return prevDmgArr;
        }

        private void ExhaustiveSearch()
        {
            double toHit = this.ToHit / 100;
            double toDef = this.ToDef / 100;
            double sum = 0;
            //var dmgArr = new double[RemaningHP + 1];
            //for (var k = 0; k < dmgArr.Length; ++k)
            //{
            //    dmgArr[k] = 0d;
            //}


            int defendingFiguresOut;
            int topDefendingFigureHPOut;
            GetRemaningFiguresHP(OneDefendingFigHP,
                RemaningHP,
                out defendingFiguresOut,
                out topDefendingFigureHPOut);
            Debug.Assert(defendingFiguresOut >=0 && defendingFiguresOut<= DefendingFigures);

            var dmgArr = CalcFiguresDamageDist
                (AttackingFigures,
                DefendingFigures,
                OneDefendingFigHP,
                topDefendingFigureHPOut);

            //int attackingFigures,
            //int defendingFigures,
            //int healthPerDefFigure,
            //int topDefendingFigureHP)

            //for (int hit = 0; hit <= Attack; ++hit)
            //{
            //    var probHit = BinomialResult(hit, toHit, Attack);
            //    for (int def = 0; def <= Defence; ++def)
            //    {
            //        var probDefence = BinomialResult(def, toDef, Defence);
            //        var dmg = hit - def <= 0 ? 0 : hit - def;
            //        dmg = dmg > RemaningHP ? RemaningHP : dmg;
            //        dmgArr[dmg] += probHit * probDefence;
            //    }
            //}


            sum = dmgArr.Sum();
            var result = new StringBuilder();
            for (var i = 0; i < dmgArr.Length; ++i)
            {
                result.Append("P(Damage = ");
                result.Append(i);
                result.Append(") = ");
                result.AppendLine(dmgArr[i].ToString());
                //result.AppendLine(String.Format("{0:0.00}", damageProb));
            }
            //}

            result.AppendLine("sum: " + String.Format("{0:0.00}", sum));
            this.Result = result.ToString();
            //}
        }


        private void EstimateButtonClicked(object sender, RoutedEventArgs e)
        {
            ExhaustiveSearch();
        }

        private double BinomialResult(int k, double p, int n)
        {
            var pPowK = Math.Pow(p, k);
            var pPowNMinK = Math.Pow(1d - p, n - k);
            var kfromN = KFromN(n, k);
            return kfromN * pPowK * pPowNMinK;
        }

        private double KFromN(int n, int k)
        {
            return (Factor(n) / Factor(n - k)) / Factor(k);
        }

        private double Factor(double n)
        {
            double res = 1d;
            for (double i = Convert.ToInt64(n); i > 1; --i)
            {
                res = res * i;
            }
            return res;
        }
    }
}
