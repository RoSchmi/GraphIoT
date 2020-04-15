﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    /// <summary>
    /// All colors used in digitalstrom.
    /// </summary>
    /// <remarks>
    /// Each group type has one of these colors assigned to them, but not each color has
    /// exactly one associated group type, especially not one single assigned group id!
    /// Blue is used for Heating (ID: 3), Cooling (ID: 9), Ventilation (ID: 10) and some more.
    /// Red and Green are not guaranteed to be only assigned to IDs 6 and 7, respectively.
    /// White is used as unknown type or single device, usually - but not restricted to - ID 0.
    /// </remarks>
    public enum Color
    {
        White = 0,
        Yellow = 1,
        Gray = 2,
        Blue = 3,
        Cyan = 4,
        Magenta = 5,
        Red = 6,
        Green = 7,
        Black = 8
    }

    /// <summary>
    /// All group types usable in digitalstrom.
    /// </summary>
    public enum GroupType
    {
        ///<summary>[Color: White] Single Device White Various, individual per device</summary>
        Various = 0,
        ///<summary>[Color: Yellow] Room lights, Garden lights, Building illuminations</summary>
        Light = 1,
        ///<summary>[Color: Gray] Blinds, Shades, Awnings, Curtains Gray Blinds</summary>
        Shading = 2,
        ///<summary>[Color: Blue] Heating actuators</summary>
        Heating = 3,
        ///<summary>[Color: Blue] Cooling actuators</summary>
        Cooling = 9,
        ///<summary>[Color: Blue] Ventilation actuators</summary>
        Ventilation = 10,
        ///<summary>[Color: Blue] Windows actuators for opening and closing</summary>
        Window = 11,
        ///<summary>[Color: Blue] Ceiling fan, Fan coil units</summary>
        AirRecirculation = 12,
        ///<summary>[Color: Blue] Ventilation system</summary>
        ApartmentVentilation = 64,
        ///<summary>[Color: Blue] Blue Single room temperature control</summary>
        TemperatureControl = 48,
        ///<summary>[Color: Cyan] Playing music or radio</summary>
        Audio = 4,
        ///<summary>[Color: Magenta] TV, Video</summary>
        Video = 5,
        ///<summary>[Color: Red] Security related functions, Alarms</summary>
        Security = 6,
        ///<summary>[Color: Green] Access related functions, door bell</summary>
        Access = 7,
        ///<summary>[Color: Black] Configurable</summary>
        Joker = 8
    }

    public class Group : IComparable, IComparable<Group>, IEquatable<Group>, IFormattable
    {
        private readonly int _group = (int)Color.Yellow;

        private Group(int colorCode)
        {
            _group = Math.Min(Math.Max(colorCode, 0), 64);
            if (_group > 12 && !(_group == 48 || _group == 64))
                _group = 0;
        }

        public static implicit operator int(Group group)
        {
            return group._group;
        }

        public static implicit operator Group(long group)
        {
            return (int)group;
        }

        public static implicit operator Group(int group)
        {
            return new Group(group);
        }

        public static implicit operator Group(string groupID)
        {
            if (!int.TryParse(groupID, out int color))
                return new Group((int)Color.White);

            return new Group(color);
        }

        public static implicit operator Group(Color color)
        {
            return new Group((int)color);
        }

        public static implicit operator Group(GroupType type)
        {
            return new Group((int)type);
        }

        public static implicit operator Color(Group group)
        {
            if (group._group <= 0)
                return Color.White;

            if ((group._group >= 9 && group._group <= 12) || group._group == 48 || group._group == 64)
                return Color.Blue;

            if (group._group > 8)
                return Color.White;

            return (Color)group._group;
        }

        public static implicit operator GroupType(Group group)
        {
            if (group._group == 48 || group._group == 64)
                return (GroupType)group._group;
            if (group._group <= 0 || group._group > 12)
                return GroupType.Various;

            return (GroupType)group._group;
        }

        public static IEnumerable<Group> GetGroups()
        {
            return ((Color[])Enum.GetValues(typeof(Color))).Select(x => (Group)x);
        }

        public static bool operator !=(Group group1, Group group2)
        {
            return !(group1 == group2);
        }

        public static bool operator ==(Group group1, Group group2)
        {
            if (group1 is null || group2 is null)
                return ReferenceEquals(group1, group2);
            return group1._group == group2._group;
        }

        public int CompareTo(Group value)
        {
            return _group.CompareTo(value._group);
        }

        public int CompareTo(object? value)
        {
            return _group.CompareTo((value as Group)?._group ?? value);
        }

        public bool Equals(Group group)
        {
            return this == group;
        }

        public override bool Equals(object? obj)
        {
            return obj is Group group && this == group;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_group);
        }

        public override string ToString()
        {
            return $"ID {_group}: {(Color)this} - {(GroupType)this}";
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">
        /// Null for an invariant default format 'ID {group-number}: {color} - {group-name}'.
        /// "D" or "d" for a localized displayable string in the format '{color} - {group-name}',
        /// if available for the given language of the format provider.
        /// </param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format and provider.
        /// </returns>
        public string ToString(string? format = null, IFormatProvider? formatProvider = null)
        {
            if (format is null)
                return $"ID {_group}: {(Color)this} - {(GroupType)this}";

            if (!format.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException($"Did not recognize format '{format}'");

            if (formatProvider is CultureInfo culture && culture.TwoLetterISOLanguageName.Equals("de", StringComparison.CurrentCultureIgnoreCase))
            {
                return _group switch
                {
                    (int)GroupType.Various => "Weiss - Gerät",
                    (int)GroupType.Light => "Gelb - Licht",
                    (int)GroupType.Shading => "Grau - Beschattung",
                    (int)GroupType.Heating => "Blau - Heizung",
                    (int)GroupType.Cooling => "Blau - Kühlung",
                    (int)GroupType.Ventilation => "Blau - Lüftung",
                    (int)GroupType.Window => "Blau - Fenster",
                    (int)GroupType.AirRecirculation => "Blau - Luftzirkulation",
                    (int)GroupType.ApartmentVentilation => "Blau - Lüftungssystem",
                    (int)GroupType.TemperatureControl => "Blau - Einzelraumregelung",
                    (int)GroupType.Audio => "Cyan - Audio",
                    (int)GroupType.Video => "Magenta - Video",
                    (int)GroupType.Security => "Rot - Sicherheit",
                    (int)GroupType.Access => "Grün - Zugang",
                    (int)GroupType.Joker => "Schwarz - Joker",
                    _ => string.Format("Unbekannt ({0})", _group),
                };
            }

            return _group switch
            {
                (int)GroupType.Various => "White - Device",
                (int)GroupType.Light => "Yellow - Light",
                (int)GroupType.Shading => "Blue - Shading",
                (int)GroupType.Heating => "Blue - Heating",
                (int)GroupType.Cooling => "Blue - Cooling",
                (int)GroupType.Ventilation => "Blue - Ventilation",
                (int)GroupType.Window => "Blue - Window",
                (int)GroupType.AirRecirculation => "Blue - Air Recirculation",
                (int)GroupType.ApartmentVentilation => "Blue - Apartment Ventilation",
                (int)GroupType.TemperatureControl => "Blue - Temperature Control",
                (int)GroupType.Audio => "Cyan - Audio",
                (int)GroupType.Video => "Magenta - Video",
                (int)GroupType.Security => "Red - Security",
                (int)GroupType.Access => "Green - Access",
                (int)GroupType.Joker => "Black - Joker",
                _ => string.Format("Unknown ({0})", _group),
            };
        }
    }
}