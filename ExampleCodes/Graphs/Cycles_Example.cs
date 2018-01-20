
using NCDK.Templates;

namespace NCDK.Graphs
{
    public class Cycles_Example1
    {
        public void Main()
        {
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region AllFinder
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // handle error - note it is common that finding all simple cycles in chemical graphs is intractable
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region MCB
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - MCB should never be intractable
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region Relevant
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - there may be an exponential number of cycles but this is not currently checked
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region Essential
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - essential cycles do not check tractability
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region TripletShort
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - triple short cycles do not check tractability
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region VertexShort
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - vertex short cycles do not check tractability
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region EdgeShort
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - edge short cycles do not check tractability
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region CDKAromaticSetFinder
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - edge short cycles do not check tractability
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region AllOrVertexShortFinder
                ICycleFinder cf = Cycles.AllFinder;
                foreach (var container in containers) 
                {
                    try
                    {
                        Cycles cycles = cf.Find(container);
                        IRingSet rings  = cycles.ToRingSet();
                    } 
                    catch (IntractableException) 
                    {
                        // ignore error - edge short cycles do not check tractability
                    }
                }
                #endregion
            }

            {
                #region Or6
                // all cycles or all cycles size <= 6
                ICycleFinder cf = Cycles.Or(Cycles.AllFinder, Cycles.GetAllFinder(6));
                #endregion
            }
            {
                #region OrARE
                // all cycles or relevant or essential
                ICycleFinder cf = Cycles.Or(Cycles.AllFinder, Cycles.Or(Cycles.RelevantFinder, Cycles.EssentialFinder));
                #endregion
            }

            {
                IChemObjectSet<IAtomContainer> containers = null;
                #region FindAll
                foreach (var container in containers)
                {
                    try
                    {
                        Cycles cycles = Cycles.FindAll(container);
                        IRingSet rings = cycles.ToRingSet();
                    }
                    catch (IntractableException)
                    {
                        // handle error - note it is common that finding all simple cycles in chemical graphs is intractable
                    }
                }
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindMCB
                        Cycles cycles = Cycles.FindMCB(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindSSSR
                        Cycles cycles = Cycles.FindSSSR(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindRelevant
                        Cycles cycles = Cycles.FindRelevant(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindEssential
                        Cycles cycles = Cycles.FindEssential(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindTripletShort
                        Cycles cycles = Cycles.FindTripletShort(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindVertexShort
                        Cycles cycles = Cycles.FindVertexShort(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
            {
                IChemObjectSet<IAtomContainer> containers = null;
                IAtomContainer container = null;
                #region FindEdgeShort
                        Cycles cycles = Cycles.FindEdgeShort(container);
                        IRingSet rings = cycles.ToRingSet();
                #endregion
            }
        }
    }
}

