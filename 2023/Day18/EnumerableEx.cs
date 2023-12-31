namespace EmnumerableEx;

public static class EnumerableExes {

    public static IEnumerable<TResult> Tripplewise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return _(); IEnumerable<TResult> _()
            {
                using var e = source.GetEnumerator();

                if (!e.MoveNext())
                    yield break;

                var previous1 = e.Current;

                if (!e.MoveNext())
                    yield break;

                var previous2 = e.Current;


                while (e.MoveNext())
                {
                    yield return resultSelector(previous1, previous2, e.Current);
                    previous1 = previous2;
                    previous2 = e.Current;
                }
            }
        }
}