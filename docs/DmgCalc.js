
        // returns the damage distribution of the given number of figures
        function CalcFiguresDamageDist( // double[] 
            toHit,// double
            toDef, // double
            Attack,// int
            Defence,// int
            attackingFigures, // int
            defendingFigures, // int
            healthPerDefFigure, // int
            topDefendingFigureHP, // int
			RemaningHP, // int
			toDefLimit,
			toDefAfterLimit)
        {
			// var sss1 = 0;
			// for (var i=0;i<=20;++i)
			// {
				// res11 = BinomialLimitedResult(i, 0.5, 20, 15, 0.3);
				// sss1 += res11;
			    // console.error('p: ' + i + " = " + res11);
			// }
		    // console.error('sum: ' + sss1);
			
            // lookup from damage done, to probablity of damage by a single attacking figure
			// is used to save previous results for improving performance 
            //var remaningHPToDmgDist = {};

            // the updated damage probalbility distribution, the result to return
            // we initialize it to zero and update it by summing over all possible events
            var prevDmgArr = GetZeroInitializedDamageDist(RemaningHP);
            prevDmgArr[0] = 1.0; // initially, before the first figure attacked, probability of zero damage is 100%

            for (var i = 1; i <= attackingFigures; ++i)
            {
                // probability of each damage after figure i has finished attacking/defense iteration
                // updated value for next iteration 'prevDmgArr'
                var updatedDmg = GetZeroInitializedDamageDist(RemaningHP);

                for (var previousDamage = 0; previousDamage < prevDmgArr.length; ++previousDamage)
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
                            // !remaningHPToDmgDist.TryGetValue(remaningHP, out currFigureDamageDist)
                            )
                        {
                            // if we can't find it in the lookup, calculate it and update the lookup
                            //currFigureDamageDist = GetZeroInitializedDamageDist(RemaningHP);

                            // get updated stepDefendingFigures, steptopDefendingFigureHP
                            var res = CalcRemaining(defendingFigures,
                                                healthPerDefFigure,
                                                topDefendingFigureHP,
                                                previousDamage);
                            var stepDefendingFigures = res.defendingFiguresOut;
                            var steptopDefendingFigureHP = res.topDefendingFigureHPOut;

                            // calculate currFigureDamageDist
                            CalcAttackStep(
                                toHit,
                                toDef,
                                Attack,
                                Defence,
                                currFigureDamageDist, // current figure damage distribution, this is the value being calculated
                                1.0,    // the probabillty of reaching this step
                                0,
                                stepDefendingFigures,
                                healthPerDefFigure,
                                steptopDefendingFigureHP,
								toDefLimit,
								toDefAfterLimit);

                            // update the lookup for future searches
                            //remaningHPToDmgDist[remaningHP] = currFigureDamageDist;
                        }

                        for (var currentFigureDmg = 0; currentFigureDmg < currFigureDamageDist.length; ++currentFigureDmg)
                        {
                            var currFigureDmgProb = currFigureDamageDist[currentFigureDmg];
                            var dmgInd = previousDamage + currentFigureDmg;
                            dmgInd = dmgInd >= updatedDmg.length ? updatedDmg.length - 1 : dmgInd;
                            updatedDmg[dmgInd] += currFigureDmgProb * previousDmgProb;
                        }
                    }
                }

                prevDmgArr = updatedDmg;
            }

            return prevDmgArr;
        }


    function CalcAttackStep(
		toHit,
		toDef,
		Attack,
		Defence,			
        dmgDist,
        probReachThisStep,
        dmgDoneSoFar,
        defendingFigures,
        healthPerDefFigure,
        topDefendingFigureHP,
		toDefLimit,
		toDefAfterLimit)
        {
            for (var hit = 0; hit <= Attack; ++hit)
            {
                var probHit = BinomialResult(hit, toHit, Attack);
                if (hit == 0)
                {
                    var dmgInd = dmgDoneSoFar;
                    dmgInd = dmgInd >= dmgDist.length ? dmgDist.length - 1 : dmgInd;
                    dmgDist[dmgInd] += probReachThisStep * probHit;
                    continue;
                }
                else
                {
                    DefenceSteps(
						toDef,
						Defence,					
                        dmgDist,
                        probReachThisStep * probHit,
                        dmgDoneSoFar,
                        hit,
                        defendingFigures,
                        healthPerDefFigure,
                        topDefendingFigureHP,
						toDefLimit,
						toDefAfterLimit);
                }
            }
        }

       function DefenceSteps(
			toDef,
			Defence,
            dmgDist, // double[] 
            probReachThisStep, // double
            dmgDoneSoFar, // int 
            attackHitsLeft, // int 
            defendingFigures, // int 
            healthPerDefFigure, // int 
            topDefendingFigureHP, // int
			toDefLimit,
			toDefAfterLimit)
        {
            for (var def = 0; def <= Defence; ++def)
            {
                //var probDefence = BinomialResult(def, toDef, Defence);
                var probDefence = BinomialLimitedResult(def, toDef, Defence, toDefLimit, toDefAfterLimit);
				
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
                        dmgInd = dmgInd >= dmgDist.length ? dmgDist.length - 1 : dmgInd;
                        dmgDist[dmgInd] += probReachThisStep * probDefence;
                        continue;
                    }
                    else
                    {
                        if (attHitsLeft == 0)
                        {
                            // no attacker hits left , finish defending steps and update probability of this event sequence
                            var dmgInd = dmgDoneSoFar + dmgDoneToFigure;
                            dmgInd = dmgInd >= dmgDist.length ? dmgDist.length - 1 : dmgInd;
                            dmgDist[dmgInd] += probReachThisStep * probDefence;
                            continue;
                        }
                        else
                        {
							Assert(attHitsLeft > 0, 'attHitsLeft must be greater than 0');
                            DefenceSteps(
								toDef,
								Defence,							
								dmgDist,
                                probReachThisStep * probDefence,
                                dmgDoneSoFar + dmgDoneToFigure,
                                attHitsLeft,
                                remaningFigures,
                                healthPerDefFigure,
                                healthPerDefFigure,
								toDefLimit,
								toDefAfterLimit);
                            continue;
                        }
                    }
                }
                else
                {
					Assert(dmgDoneToFigure < topDefendingFigureHP && dmgDoneToFigure >= 0, 'dmgDoneToFigure < topDefendingFigureHP && dmgDoneToFigure >= 0');
                    // did not kill top figure, stop defence steps and update probability of this event sequence
                    var dmgInd = dmgDoneSoFar + dmgDoneToFigure;
                    dmgInd = dmgInd >= dmgDist.length ? dmgDist.length - 1 : dmgInd;
                    dmgDist[dmgInd] += probReachThisStep * probDefence;
                    continue;
                }
            }
        }
        
		
		// the probability to have k successes in n attempts when the chance of success is p
		function BinomialResult(k, p, n)
        {
            var pPowK = Math.pow(p, k);
            var pPowNMinK = Math.pow(1 - p, n - k);
            var kfN = KFromN(n, k);
            return kfN * pPowK * pPowNMinK;
        }
		
		function BinomialLimitedResult(k, p, n, limit, q)
        {
			if (n <= limit || p <= q)
			{
				return BinomialResult(k, p, n);;
			}
			else // n > limit 
			{
				var sum = 0;
				// in the p part we will have number of successes not greather than limit
				var pPartMax = k > limit ? limit : k;
				// in the p part minimal number of successes not smaller than k-(n-limit)
				// example: 17 defense 16 successes: minimum number of defense with p=50% is 14 = 16-(17-15)
				var pPartMin = k-(n-limit) < 0 ? 0 : k-(n-limit);
				
				// i = how many successes were in the p part
				for (var i= pPartMin ; i <= pPartMax ; ++i)
				{
					pPart = BinomialResult(i, p, limit);
					qPart = BinomialResult(k-i, q, n-limit);
					sum += pPart*qPart;
				}
				return sum;
			}
        }

		// the number of distinct ways to place k items in n slots
		// https://en.wikipedia.org/wiki/Binomial_coefficient
        function KFromN(n, k)
        {
            return (Factor(n) / Factor(n - k)) / Factor(k);
        }

		// returns the factorial of n
		// https://en.wikipedia.org/wiki/Factorial
        function Factor(n)
        {
            var res = 1;
            for (var i = n; i > 1; --i)
            {
                res = res * i;
            }
            return res;
        }
		
        function Assert(val, errMsg)
        {
			if (val !== true)
			{
			    console.error('Error: ' + errMsg);
			}
        }		

        function GetZeroInitializedDamageDist(
			remaningHP// int 
			)
        {
            var dmgArr = [];
            for (var k = 0; k < (remaningHP + 1); ++k)
            {
                dmgArr[k] = 0;
            }
            return dmgArr;
        }

        function GetRemaningHP(
            defendingFigures, // int
            healthPerDefFigure, // int
            topDefendingFigureHP) // int
        {
            var result = (defendingFigures - 1) * healthPerDefFigure + topDefendingFigureHP;
            Assert(result >= 0);
            return result;
        }

        // given the initial stack stats and the damage already done,
        // returns the updated number of defending figures and
        // the hitpoints of the top defending figure
        function CalcRemaining( // out int defendingFiguresOut
                                // out int topDefendingFigureHPOut
                            defendingFigures, // int
                            healthPerDefFigure, // int
                            topDefendingFigureHP, // int
                            dmgDone) // int
        {
            Assert(defendingFigures > 0);
            Assert(healthPerDefFigure > 0);
            Assert(topDefendingFigureHP > 0);
            Assert(dmgDone >= 0);
            var remainingDmg = GetRemaningHP(defendingFigures, healthPerDefFigure, topDefendingFigureHP) - dmgDone;
            var topDefendingFigureHPOut = remainingDmg % healthPerDefFigure;
            topDefendingFigureHPOut = topDefendingFigureHPOut == 0 ? healthPerDefFigure : topDefendingFigureHPOut;
            var defendingFiguresOut = ((remainingDmg - topDefendingFigureHPOut) / healthPerDefFigure)+1;
            Assert(
                ((defendingFigures - 1) * healthPerDefFigure + topDefendingFigureHP)
                == ((defendingFiguresOut - 1) * healthPerDefFigure + topDefendingFigureHPOut + dmgDone));
            Assert(defendingFiguresOut >= 0);
            Assert(topDefendingFigureHPOut >= 0);

            return {
                defendingFiguresOut: defendingFiguresOut,
                topDefendingFigureHPOut: topDefendingFigureHPOut
            }
        }

