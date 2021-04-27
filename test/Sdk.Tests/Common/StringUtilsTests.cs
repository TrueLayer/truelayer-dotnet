using System.Collections;
using System.Collections.Generic;
using TrueLayerSdk.Common;
using Xunit;

namespace TrueLayer.Sdk.Tests.Common
{
    public class StringUtilsTests
    {
        [Theory]
        [ClassData(typeof(SnakeCaseDataGenerator))]
        public void ToSnakeCaseTest(string expected, string actual) => Assert.Equal(expected, actual.ToSnakeCase());
    }

    public class SnakeCaseDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 50, 50 };
            yield return new object[] { "url_value", "URLValue" };
            yield return new object[] { "url", "URL" };
            yield return new object[] { "id", "ID" };
            yield return new object[] { "i", "I" };
            yield return new object[] { "", "" };
            yield return new object[] { null, null };
            yield return new object[] { "person", "Person" };
            yield return new object[] { "i_phone", "iPhone" };
            yield return new object[] { "i_phone", "IPhone" };
            yield return new object[] { "i_phone", "I Phone" };
            yield return new object[] { "i_phone", "I  Phone" };
            yield return new object[] { "i_phone", " IPhone" };
            yield return new object[] { "i_phone", " IPhone " };
            yield return new object[] { "is_cia", "IsCIA" };
            yield return new object[] { "vm_q", "VmQ" };
            yield return new object[] { "xml2_json", "Xml2Json" };
            yield return new object[] { "sn_ak_ec_as_e", "SnAkEcAsE" };
            yield return new object[] { "sn_a__k_ec_as_e", "SnA__kEcAsE" };
            yield return new object[] { "sn_a__k_ec_as_e", "SnA__ kEcAsE" };
            yield return new object[] { "already_snake_case_", "already_snake_case_ " };
            yield return new object[] { "is_json_property", "IsJSONProperty" };
            yield return new object[] { "shouting_case", "SHOUTING_CASE" };
            yield return new object[] { "9999-12-31_t23:59:59.9999999_z", "9999-12-31T23:59:59.9999999Z" };
            yield return new object[] { "hi!!_this_is_text._time_to_test.", "Hi!! This is text. Time to test." };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
