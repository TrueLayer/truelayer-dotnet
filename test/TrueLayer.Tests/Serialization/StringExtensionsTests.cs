using System.Collections.Generic;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization;

public class StringExtensionsTests
{
    [Theory]
    [MemberData(nameof(SnakeCaseData))]
    public void Can_convert_to_snake_case(string expected, string actual) => Assert.Equal(expected, actual.ToSnakeCase());

    public static IEnumerable<object[]> SnakeCaseData()
    {
        var snakeCaseSamples = new List<object[]>
        {
            new object[] {50, 50},
            new object[] {"url_value", "URLValue"},
            new object[] {"url", "URL"},
            new object[] {"id", "ID"},
            new object[] {"i", "I"},
            new object[] {"", ""},
            new object[] {"person", "Person"},
            new object[] {"i_phone", "iPhone"},
            new object[] {"i_phone", "IPhone"},
            new object[] {"i_phone", "I Phone"},
            new object[] {"i_phone", "I  Phone"},
            new object[] {"i_phone", " IPhone"},
            new object[] {"i_phone", " IPhone "},
            new object[] {"is_cia", "IsCIA"},
            new object[] {"vm_q", "VmQ"},
            new object[] {"xml2_json", "Xml2Json"},
            new object[] {"sn_ak_ec_as_e", "SnAkEcAsE"},
            new object[] {"sn_a__k_ec_as_e", "SnA__kEcAsE"},
            new object[] {"sn_a__k_ec_as_e", "SnA__ kEcAsE"},
            new object[] {"already_snake_case_", "already_snake_case_ "},
            new object[] {"is_json_property", "IsJSONProperty"},
            new object[] {"shouting_case", "SHOUTING_CASE"},
            new object[] {"9999-12-31_t23:59:59.9999999_z", "9999-12-31T23:59:59.9999999Z"},
            new object[] {"hi!!_this_is_text._time_to_test.", "Hi!! This is text. Time to test."},
        };

        return snakeCaseSamples;
    }
}