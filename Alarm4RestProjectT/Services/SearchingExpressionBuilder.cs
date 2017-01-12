﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Alarm4Rest.Data;
using System.Reflection;

namespace Alarm4Rest_Viewer.Services
{
    public class SearchingExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] {typeof(string) });
        private static MethodInfo toUpperMethod = typeof(string).GetMethod("ToUpper", new[] { typeof(string) });
        

        /*Methode that solve Many List field 
         Please gouping likt this -->IEnumerable<IGrouping<string, Item>> gruopFields =
                                            from item in filters
                                            group item by item.FieldName;
         
         before call this methode:
         */
        public static Expression<Func<T, bool>> GetExpression<T>(IEnumerable<IGrouping<string, Item>> groupFilters,string keyWord)
        {
            Expression outerExp; //And between field

            outerExp = null;
            ParameterExpression pe = Expression.Parameter(typeof(T), "RestorationAlarm");

            try
            {

                foreach (IGrouping<string, Item> groupfield in groupFilters)
                {
                    
                    List<Item> gruopfields = groupfield.ToList();

                    if (gruopfields.Count == 0) return null;

                    Expression innerExp = null; //Or in the same field
                    
                    //If it is SearchingField Group
                    if (groupfield.Key == "FieldName")
                    {
                        
                        // has 1 item parameter
                        if (gruopfields.Count == 1)
                            innerExp = GetExpression<T>(pe, gruopfields[0], keyWord, groupfield.Key); //Create expression from a single instance
                    
                        // has 2 item parameter
                        else if (gruopfields.Count == 2)

                            innerExp = GetExpression<T>(pe, gruopfields[0], gruopfields[1], keyWord, groupfield.Key); //Create expression that utilizes OrElse mentality
                    
                        // More than 2 items parameter
                        else
                        {  
                            //Loop through filters until we have create an expression for each 
                            while (gruopfields.Count > 0)
                            {
                                var f1 = gruopfields[0];
                                var f2 = gruopfields[1];

                                if (innerExp == null) //First time
                                    innerExp = GetExpression<T>(pe, gruopfields[0], gruopfields[1], keyWord, groupfield.Key);
                                else //Not First time
                                    innerExp = Expression.OrElse(innerExp, GetExpression<T>(pe, gruopfields[0], gruopfields[1], keyWord, groupfield.Key));

                                gruopfields.Remove(f1);
                                gruopfields.Remove(f2);

                                if (gruopfields.Count == 1)
                                {
                                    innerExp = Expression.OrElse(innerExp, GetExpression<T>(pe, gruopfields[0], keyWord, groupfield.Key));
                                    gruopfields.RemoveAt(0);
                                }
                            }
                        }
                    }else if (groupfield.Key == "StationName")
                    {
                        // has 1 item parameter
                        if (gruopfields.Count == 1)
                            innerExp = GetExpression<T>(pe, gruopfields[0]); //Create expression from a single instance

                        // has 2 item parameter
                        else if (gruopfields.Count == 2)

                            innerExp = GetExpression<T>(pe, gruopfields[0], gruopfields[1]); //Create expression that utilizes OrElse mentality

                        // More than 2 items parameter
                        else
                        {
                            //Loop through filters until we have create an expression for each 
                            while (gruopfields.Count > 0)
                            {
                                var f1 = gruopfields[0];
                                var f2 = gruopfields[1];

                                if (innerExp == null) //First time
                                    innerExp = GetExpression<T>(pe, gruopfields[0], gruopfields[1]);
                                else //Not First time
                                    innerExp = Expression.OrElse(innerExp, GetExpression<T>(pe, gruopfields[0], gruopfields[1]));

                                gruopfields.Remove(f1);
                                gruopfields.Remove(f2);

                                if (gruopfields.Count == 1)
                                {
                                    innerExp = Expression.OrElse(innerExp, GetExpression<T>(pe, gruopfields[0]));
                                    gruopfields.RemoveAt(0);
                                }
                            }
                        }

                    }    

                    if (outerExp == null)
                    {
                        outerExp = innerExp;
                    }
                    else
                    {
                        outerExp = PredicateBuilder.AndT(innerExp, outerExp);

                    }

                    Console.WriteLine("{0}", groupfield.Key);

                } // Next field Group

                //Finishing
                return Expression.Lambda<Func<T, bool>>(outerExp, pe);
            }
            catch{
                return null;
            }
        }  

        //Helper

        //For 1 parameter
        private static Expression GetExpression<T>(ParameterExpression pe, Item filter)
        {
            MemberExpression me = Expression.Property(pe, filter.FieldName); //change to variable
            ConstantExpression constant = Expression.Constant(filter.Value.TrimEnd());
            return Expression.Call(me, containsMethod, constant);
        }

        private static Expression GetExpression<T>(ParameterExpression pe, Item filter, string keyWord, string memberExp)
        {
            MemberExpression me1 = null;
            ConstantExpression constant1 = null;

            switch (memberExp)
            {
                case "FieldName":
                     me1 = Expression.Property(pe, filter.Value.TrimEnd()); //change to variable
                     constant1 = Expression.Constant(keyWord);
                    break;
                case "StationName":
                     me1 = Expression.Property(pe, filter.FieldName); //change to variable
                     constant1 = Expression.Constant(filter.Value.TrimEnd());
                    break;
                default:
                    break;
            }

            //Expression member = Expression.Call(me, typeof(string).GetMethod("ToUpper", System.Type.EmptyTypes));
            //return Expression.Call(member, containsMethod, constant);

            return Expression.Call(me1, containsMethod, constant1);
        }


        //For 2 parameter
        private static Expression GetExpression<T>(ParameterExpression pe, Item filter1, Item filter2)
        {

            Expression result1 = GetExpression<T>(pe, filter1);
            Expression result2 = GetExpression<T>(pe, filter2);

            return Expression.OrElse(result1, result2);
        }

        private static Expression GetExpression<T>(ParameterExpression pe, Item filter1, Item filter2, string keyWord, string memberExp)
        {

            Expression result1 = GetExpression<T>(pe, filter1, keyWord, memberExp);
            Expression result2 = GetExpression<T>(pe, filter2, keyWord, memberExp);

            return Expression.OrElse(result1, result2);
        }
    }

}
