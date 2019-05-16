using Runes.Collections;
using Runes.Collections.Immutable;
using System;
using System.Collections.Generic;

using static Runes.Collections.Immutable.Streams;
using static Runes.Options;
using static Runes.Lazies;
using static Runes.Tries;
using static Runes.Units;

namespace Runes
{
    public abstract class Try<A> : Traversable<A>
    {
        public bool IsSuccess => GetIfSuccess(out A _);
        public bool IsFailure => GetIfFailure(out Exception _);

        public Try<B> ContinueWith<B>(Func<A, Try<B>> f)
        {
            switch (this)
            {
                case Success<A> succ:
                    return f(succ.Result);

                case Failure<A> fail:
                    return Failure<B>(fail.Exception);

                default:
                    throw new CodeNeverShouldBeReachedException();
            }
        }
        public abstract Try<B> FlatMap<B>(Func<A, Try<B>> f);
        public override Unit Foreach(Action<A> action) => Unit(() =>
        {
            if(GetIfSuccess(out A value))
                action(value);
        });
        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
        {
            if(GetIfSuccess(out A value) && p(value))
                action(value);
        });
        public override IEnumerator<A> GetEnumerator()
        {
            if (GetIfSuccess(out A res))
            {
                yield return res;
            }
        }
        public bool GetIfFailure(out Exception ex) => GetIfFailure<Exception>(out ex);
        public abstract bool GetIfFailure<E>(out E ex) where E: Exception;
        public abstract bool GetIfSuccess(out A result);
        public abstract Try<B> Map<B>(Func<A, B> f);
        public Try<A> OrElse(Try<A> alternative) => IsSuccess ? this : alternative;
        public Try<A> OrElse(Lazy<Try<A>> alternative) => IsSuccess ? this : alternative;
        public Try<A> RecoverWith<E>(Func<E, A> recoverFunc) where E: Exception =>
            GetIfFailure(out E ex) ? Try(() => recoverFunc(ex)) : this;
        public Try<A> RecoverWith<E>(Func<E, Try<A>> recoverFunc) where E: Exception =>
            GetIfFailure(out E ex) ? recoverFunc(ex) : this;
        public Option<A> ToOption() => GetIfSuccess(out A result) ? Some(result) : None<A>();
        public override Stream<A> ToStream() => GetIfSuccess(out A res) ? Stream(res) : Empty<A>();
        public Try<B> Transform<B>(Func<A, Try<B>> successTranformation, Func<Exception, Try<B>> failureTransformation)
        {
            switch (this)
            {
                case Success<A> succ:
                    return successTranformation(succ.Result);

                case Failure<A> fail:
                    return failureTransformation(fail.Exception);

                default:
                    throw new CodeNeverShouldBeReachedException();
            }
        }

        private protected Try() { }
    }

    public sealed class Success<A> : Try<A>
    {
        public A Result { get; }

        public override Try<B> FlatMap<B>(Func<A, Try<B>> f)
        {
            try
            {
                return f(Result);
            }
            catch(Exception ex)
            {
                return Failure<B>(ex);
            }
        }

        public override bool GetIfSuccess(out A result)
        {
            result = Result;
            return true;
        }

        public override bool GetIfFailure<E>(out E ex)
        {
            ex = default;
            return false;
        }

        public override Try<B> Map<B>(Func<A, B> f) => Try(() => f(Result));

        public override string ToString()
        {
            return $"Success({Result})";
        }

        public void Deconstruct(out A value)
        {
            value = Result;
        }

        internal Success(A result)
        {
            Result = result;
        }
    }

    public sealed class Failure<A> : Try<A>
    {
        public Exception Exception { get; }

        public Failure<B> As<B>() => Failure<B>(Exception);

        public override Try<B> FlatMap<B>(Func<A, Try<B>> f) => As<B>();

        public override bool GetIfSuccess(out A result)
        {
            result = default;
            return false;
        }

        public override bool GetIfFailure<E>(out E ex)
        {
            ex = Exception as E;
            return ex == Exception;
        }

        public override Try<B> Map<B>(Func<A, B> f) => As<B>();

        public override string ToString()
        {
            return $"Failure({Exception})";
        }

        public void Deconstruct(out Exception ex)
        {
            ex = Exception;
        }

        internal Failure(Exception ex)
        {
            Exception = ex;
        }
    }

    public static class Tries
    {
        public static Try<Unit> Try(Action action) => Try(() => Unit(action));
        public static Try<A> Try<A>(Lazy<A> lazy)
        {
            try
            {
                var res = lazy.Get();
                return Success(res);
            }
            catch(Exception ex)
            {
                return Failure<A>(ex);
            }
        }
        public static Try<A> Try<A>(Func<A> func) => Try(Lazy(func));

        public static bool IsFailure<A, E>(Func<A> func, out E ex) where E: Exception => Try(func).GetIfFailure(out ex);
        public static bool IsSuccess<A>(Func<A> func, out A result) => Try(func).GetIfSuccess(out result);

        internal static Failure<A> Failure<A>(Exception ex) => new Failure<A>(ex);
        internal static Success<A> Success<A>(A result) => new Success<A>(result);
    }
}
