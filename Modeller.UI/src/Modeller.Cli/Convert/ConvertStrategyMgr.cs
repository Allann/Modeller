using System;
using System.Collections.Generic;
using Core.Convert.Strategies;

namespace Core.Convert
{
    /// <summary>
    /// Manages the strategies that can be used to convert values. Used by ConvertEx.
    /// </summary>
    public static class ConvertStrategyMgr
    {
        static ConvertStrategyMgr()
        {
            sStrategies = new List<IConvertStrategy>
            {
                new NoConvertConversionStrategy(),
                new StringToBoolConversionStrategy(),
                new StaticMethodConversionStrategy(),
                new SqlDBTypeConversionStrategy(),
                new EnumConversionStrategy(),
                new ByteArrayStringConversionStrategy(),
                new TypeConverterConversionStrategy(),
                new ConvertMethodConversionStrategy(),
                new CtorConversionStrategy(),
                new ConvertibleConversionStrategy()
            };
        }

        private static readonly IConvertStrategy sNullConverter = new DefaultValueConversionStrategy();
        private static readonly List<IConvertStrategy> sStrategies;

        /// <summary>
        /// Gets a conversion strategy based on the from and to types.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IConvertStrategy GetStrategy(Type from, Type to)
        {
            if (sNullConverter.CanConvert(from, to)) return sNullConverter;
            foreach (var strategy in sStrategies)
            {
                if (strategy.CanConvert(from, to)) 
                    return strategy;
            }
            return null;
        }

        /// <summary>
        /// Register a custom strategy. Custom strategies will be considered before built-in strategies.
        /// </summary>
        /// <param name="strategy"></param>
        public static void RegisterStrategy(IConvertStrategy strategy)
        {
            sStrategies.Insert(0, strategy);
        }
    }
}
