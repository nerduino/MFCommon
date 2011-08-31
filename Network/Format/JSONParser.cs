using System;
using System.Collections;
using System.IO;

namespace MFCommon.Network {
    public class JSONParser : IDisposable {
        private Char[] _accumulator;
        private Char[] _unicodeAccumulator;
        private Stack _stack;

        private bool _inString;
        private bool _inStringSlash;
        private bool _inValue;
        private bool _inUnicodeCharacter;
        private int _accumulatorIndex;
        private int _unicodeCharacterIndex;
        
        public JSONParser(int maxStringCapacity = 512) {
            AutoCasting = true;
            _accumulator = new Char[maxStringCapacity + 1];
            _unicodeAccumulator = new Char[4];
            _stack = new Stack();
        }

        public bool AutoCasting { get; set; }

        public Hashtable Parse(StreamReader stream) {
            var length = stream.BaseStream.Length;
            while (length-- > 0) {
                ProcessCharacter((Char) stream.Read());
            }

            var jsonObject = Pop();
            return (Hashtable) jsonObject.Object;
        }

        public Hashtable Parse(string jsonText) {
            foreach (Char c in jsonText) {
                ProcessCharacter(c);
            }

            var jsonObject = Pop();
            return (Hashtable) jsonObject.Object;
        }

        protected void ProcessCharacter(Char c) {
            switch (c) {
                case '{':
                    Push(new Hashtable(), JSONObjectType.Object);
                    return;
                case '}':
                    Push(_accumulator, JSONObjectType.String);
                    Store();
                    _inValue = false;
                    return;
                case '[':
                    Push(new ArrayList(), JSONObjectType.Array);
                    return;
                case ']':
                    Push(_accumulator, JSONObjectType.String);
                    Store();
                    _inValue = false;
                    return;
                case '"':
                    if (!_inString) {
                        _inString = true;
                    } else {
                        if (_inStringSlash) {
                            Accumulate(c);
                            _inStringSlash = false;
                        } else {
                            Push(_accumulator, JSONObjectType.String);
                            _inString = false;
                        }
                    }
                    return;
                case '\\':
                    if (_inString) {
                        if (_inUnicodeCharacter) {
                            Accumulate(ConvertHexDigit());
                            _inUnicodeCharacter = false;
                        } 
                        if (_inStringSlash) {
                            Accumulate(c);
                            _inStringSlash = false;
                        } else {
                            _inStringSlash = true;
                        }
                    }
                    return;
                case 'b':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate('\b');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'f':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate('\f');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'n':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate('\n');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'r':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate('\r');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 't':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate('\t');
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case 'u':
                    if (_inString) {
                        if (_inStringSlash) {
                            _inUnicodeCharacter = true;
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case '/':
                    if (_inString) {
                        if (_inStringSlash) {
                            Accumulate(c);
                            _inStringSlash = false;
                            return;
                        }
                    }
                    break;
                case ':':
                    if (!_inString) {
                        Push(_accumulator, JSONObjectType.String);
                        _inValue = true;
                        return;
                    }
                    break;
                case ',':
                    if (!_inString) {
                        Push(_accumulator, JSONObjectType.String);
                        Store();
                        _inValue = false;
                        return;
                    }
                    break;
                case ' ':
                    if (!_inString) {
                        return;
                    }
                    break;
            }

            if (_inValue || _inString) {
                if (_inUnicodeCharacter) {
                    AccumulateUnicodeCharacter(c);
                } else {
                    Accumulate(c);
                }
            }
        }

        protected Char ConvertHexDigit() {
            short hexCharValue = 0;
            int index = 0;
            while (index < _unicodeCharacterIndex) {
                hexCharValue <<= 4; 
                Char tempChar = _unicodeAccumulator[index++];
                if (tempChar >= '0' && tempChar <= '9') {
                    hexCharValue |= (short)(tempChar - '0');
                } else if (tempChar >= 'a' && tempChar <= 'f') {
                    hexCharValue |= (short)((tempChar - 'a') + 10);
                } else if (tempChar >= 'A' && tempChar <= 'F') {
                    hexCharValue |= (short)((tempChar - 'A') + 10);
                } else {
                    throw new IndexOutOfRangeException("tempChar");
                }
            }

            _unicodeCharacterIndex = 0;

            return (Char)hexCharValue;
        }

        public void Dispose() {
            _stack = null;
            _accumulator = null;
            _unicodeAccumulator = null;
        }

        protected void Push(Object obj, JSONObjectType type) {
            _stack.Push(new JSONObject(obj, type));
        }

        protected void Push(Char[] characters, JSONObjectType type) {
            if (_accumulatorIndex > 0) {
                _accumulator[_accumulatorIndex] = (Char)0;
                _stack.Push(new JSONObject(new string(characters, 0, _accumulatorIndex), type));
                _accumulatorIndex = 0;
            }
        }

        protected JSONObject Pop() {
            return (JSONObject) _stack.Pop();
        }

        protected void Store() {
            var jsonObject = (JSONObject)_stack.Peek();

            if (jsonObject.ObjectType == JSONObjectType.Object) {
                var valueObject = Pop();
                
                jsonObject = (JSONObject)_stack.Peek();

                if (jsonObject.ObjectType == JSONObjectType.String) {
                    var jsonObjectName = Pop();
                    jsonObject = (JSONObject)_stack.Peek();
                    var hashTable = (Hashtable)jsonObject.Object;
                    hashTable.Add(jsonObjectName.Object, valueObject.Object);
                    return;
                }
                if (jsonObject.ObjectType == JSONObjectType.Array) {
                    var arrayList = (ArrayList)jsonObject.Object;
                    arrayList.Add(valueObject.Object);
                    return;
                }
                throw new InvalidOperationException("Expected string key or array");
            }

            if (jsonObject.ObjectType == JSONObjectType.Array) {
                var valueObject = Pop();
                jsonObject = (JSONObject)_stack.Peek();
                if (jsonObject.ObjectType == JSONObjectType.String) {
                    var jsonObjectName = Pop();
                    jsonObject = (JSONObject)_stack.Peek();
                    var hashTable = (Hashtable)jsonObject.Object;
                    hashTable.Add(jsonObjectName.Object, valueObject.Object);
                    return;
                }
                throw new InvalidOperationException("Expected string key");
            }

            if (jsonObject.ObjectType == JSONObjectType.String) {
                var valueObject = Pop();
                jsonObject = (JSONObject)_stack.Peek();
                if (jsonObject.ObjectType == JSONObjectType.String) {
                    var jsonObjectName = Pop();
                    jsonObject = (JSONObject)_stack.Peek();
                    var hashTable = (Hashtable)jsonObject.Object;
                    hashTable.Add(jsonObjectName.Object,
                                  AutoCasting ? AutoCast((string) valueObject.Object) : valueObject.Object);
                    return;
                }
                if (jsonObject.ObjectType == JSONObjectType.Array) {
                    var arrayList = (ArrayList)jsonObject.Object;
                    arrayList.Add(valueObject.Object);
                    return;
                }
                throw new InvalidOperationException("Expected string key or array");
            }
        }

        protected Object AutoCast(string data) {
            if (data.ToLower() == "null") {
                return null;
            }
            if (data.ToLower() == "true") {
                return true;
            }
            if (data.ToLower() == "false") {
                return false;
            }

            if (IsNumeric(data)) {
                try {
                    var numericValue = Double.Parse(data);
                    return numericValue;
                }
                catch (Exception) {
                    // Do nothing
                }
            }

            return data;
        }

        protected bool IsNumeric(string data) {
            foreach (Char c in data) {
                if (c >= '0' && c <= '9' || c == '-' || c == '+' || c == 'E' || c == 'e' || c == '.') {
                    continue;
                }
                return false;
            }
            return true;
        }

        protected void Accumulate(Char c) {
            if (_accumulatorIndex < _accumulator.Length) {
                _accumulator[_accumulatorIndex++] = c;
            } else {
                throw new ArgumentOutOfRangeException("c");
            }
        }

        protected void AccumulateUnicodeCharacter(Char c) {
            if (_unicodeCharacterIndex < _unicodeAccumulator.Length) {
                _unicodeAccumulator[_unicodeCharacterIndex++] = c;
                if (_unicodeCharacterIndex == _unicodeAccumulator.Length) {
                    Accumulate(ConvertHexDigit());
                    _inUnicodeCharacter = false;
                    _unicodeCharacterIndex = 0;
                }
            } else {
                throw new ArgumentOutOfRangeException("c");
            }
        }

        public bool Find(string key, Hashtable hashTable, out Hashtable searchResult) {
            if (hashTable.Contains(key)) {
                searchResult = (Hashtable)hashTable[key];
                return true;
            }
            searchResult = null;
            return false;
        }

        public bool Find(string name, Hashtable hashTable, out ArrayList searchResult) {
            if (hashTable.Contains(name)) {
                searchResult = (ArrayList)hashTable[name];
                return true;
            }
            searchResult = null;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out string searchResult) {
            if (hashTable.Contains(key)) {
                searchResult = (string)hashTable[key];
                return true;
            }
            searchResult = null;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out bool searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (bool)hashTable[key];
                    return true;
                }
                var result = (string)hashTable[key];
                if (result == "true") {
                    searchResult = true;
                    return true;
                }
                if (result == "false") {
                    searchResult = false;
                    return true;
                }
            }
            searchResult = false;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out Double searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (Double)hashTable[key];
                } else {
                    searchResult = Double.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0.0;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out float searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (float)((Double)hashTable[key]);
                } else {
                    searchResult = (float)Double.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0.0f;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out Int16 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (Int16)((Double)hashTable[key]);
                } else {
                    searchResult = Int16.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
        
        public bool Find(string key, Hashtable hashTable, out Int32 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (Int32)((Double)hashTable[key]);
                } else {
                    searchResult = Int32.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out Int64 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (Int64)((Double)hashTable[key]);
                } else {
                    searchResult = Int64.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out UInt16 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (UInt16)((Double)hashTable[key]);
                } else {
                    searchResult = UInt16.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out UInt32 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (UInt32)((Double)hashTable[key]);
                } else {
                    searchResult = UInt32.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }

        public bool Find(string key, Hashtable hashTable, out UInt64 searchResult) {
            if (hashTable.Contains(key)) {
                if (AutoCasting) {
                    searchResult = (UInt64)((Double)hashTable[key]);
                } else {
                    searchResult = UInt64.Parse((string)hashTable[key]);
                }
                return true;
            }
            searchResult = 0;
            return false;
        }
    }
}
