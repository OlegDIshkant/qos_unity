using System;


namespace CommonTools
{
    /// <summary>
    /// Нормализованное значение от 0 до 1.
    /// </summary>
    public struct NormValue
    {
        private float _value;

        public float AsFloat => _value;

        public static NormValue Max => new NormValue(1f);
        public static NormValue Min => new NormValue(0f);

        public NormValue(float value)
        {
            _value = Math.Max(0, Math.Min(1, value));
        }

        public static bool operator ==(NormValue valA, NormValue valb)
        {
            return valA._value == valb._value;
        }

        public static bool operator !=(NormValue valA, NormValue valb)
        {
            return valA._value != valb._value;
        }

        public override string ToString() => $"НОРМАЛИЗОВАННОЕ ВРЕМЯ ({(int)(_value * 100)} %)";


        public override bool Equals(object obj)
        {
            return obj is NormValue value &&
                   _value == value._value;
        }


        public override int GetHashCode()
        {
            int hashCode = 469314756;
            hashCode = hashCode * -1521134295 + _value.GetHashCode();
            return hashCode;
        }
    }



    public static class NormValueExtensions
    {
        public static NormValue SqueezeToCenter(this NormValue normValue, float squeezeFactor)
        {
            if (squeezeFactor < 0f || squeezeFactor >= 0.5f)
            {
                throw new InvalidOperationException($"Squeeze factor is invalid '{squeezeFactor}'.");
            }

            var normTime = normValue.AsFloat;

            if (normTime < squeezeFactor) return NormValue.Min;
            if (normTime > NormValue.Max.AsFloat - squeezeFactor) return NormValue.Max;

            var changeRange = NormValue.Max.AsFloat - 2 * squeezeFactor;
            var change = normTime - squeezeFactor;
            return new NormValue(change / changeRange);

        }
    }
}
