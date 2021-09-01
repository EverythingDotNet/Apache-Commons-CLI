#region Apache License
/*
  Licensed to the Apache Software Foundation (ASF) under one or more
  contributor license agreements.  See the NOTICE file distributed with
  this work for additional information regarding copyright ownership.
  The ASF licenses this file to You under the Apache License, Version 2.0
  (the "License"); you may not use this file except in compliance with
  the License.  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 */
#endregion

namespace Apache.Commons.Cli {

    using System;

    /**
     * OptionBuilder allows the user to create Options using descriptive methods.
     * <p>
     * Details on the Builder pattern can be found at
     * <a href="http://c2.com/cgi-bin/wiki?BuilderPattern">http://c2.com/cgi-bin/wiki?BuilderPattern</a>.
     * <p>
     * This class is NOT thread safe. See <a href="https://issues.apache.org/jira/browse/CLI-209">CLI-209</a>
     *
     * @since 1.0
     * @deprecated since 1.3, use {@link Option#builder(String)} instead
     */
    [Obsolete]
    public sealed class OptionBuilder {

        /** option builder instance */
        private static readonly InternalOptionBuilder INSTANCE = new InternalOptionBuilder();


        static OptionBuilder() {
            // ensure the consistency of the initial values
            Reset();
        }


        /**
         * private constructor to prevent instances being created
         */
        private OptionBuilder() {
            // hide the constructor
        }


        /**
         * Resets the member variables to their default values.
         */
        private static void Reset() {
            INSTANCE.Reset();
        }


        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder WithLongOpt(string newLongopt) {
            INSTANCE.longopt = newLongopt;

            return INSTANCE;
        }


        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasArg() {
            INSTANCE.numberOfArgs = 1;

            return INSTANCE;
        }


        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasArg(bool hasArg) {
            INSTANCE.numberOfArgs = hasArg ? 1 : Option.UNINITIALIZED;

            return INSTANCE;
        }


        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder WithArgName(string name) {
            INSTANCE.argName = name;

            return INSTANCE;
        }


        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder IsRequired() {
            INSTANCE.required = true;

            return INSTANCE;
        }


        /**
         * The next Option created uses <code>sep</code> as a means to
         * separate argument values.
         * <p>
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator('=')
         *                           .create('D');
         *
         * String args = "-Dkey=value";
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);  // will be "key"
         * String propertyValue = opt.getValue(1); // will be "value"
         * </pre>
         *
         * @param sep The value separator to be used for the argument values.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder WithValueSeparator(char sep) {
            INSTANCE.valuesep = sep;

            return INSTANCE;
        }


        /**
         * The next Option created uses '<code>=</code>' as a means to
         * separate argument values.
         *
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator()
         *                           .create('D');
         *
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);
         * String propertyValue = opt.getValue(1);
         * </pre>
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder WithValueSeparator() {
            INSTANCE.valuesep = '=';

            return INSTANCE;
        }


        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder IsRequired(bool newRequired) {
            INSTANCE.required = newRequired;

            return INSTANCE;
        }


        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasArgs() {
            INSTANCE.numberOfArgs = Option.UNLIMITED_VALUES;

            return INSTANCE;
        }


        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasArgs(int num) {
            INSTANCE.numberOfArgs = num;

            return INSTANCE;
        }


        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasOptionalArg() {
            INSTANCE.numberOfArgs = 1;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }


        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasOptionalArgs() {
            INSTANCE.numberOfArgs = Option.UNLIMITED_VALUES;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }


        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder HasOptionalArgs(int numArgs) {
            INSTANCE.numberOfArgs = numArgs;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }


        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         * <p>
         * <b>Note:</b> this method is kept for binary compatibility and the
         * input type is supposed to be a {@link Class} object.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @deprecated since 1.3, use {@link #withType(Class)} instead
         */
        [Obsolete]
        public static IOptionBuilder WithOptionType(object newType) {
            return WithOptionType((Type)newType);
        }


        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        public static IOptionBuilder WithOptionType(Type newType) {
            INSTANCE.type = newType;

            return INSTANCE;
        }


        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder WithDescription(string newDescription) {
            INSTANCE.description = newDescription;

            return INSTANCE;
        }


        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public static Option Create(char opt) {
            return Create(Convert.ToString(opt));
        }


        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        public static Option Create() {
            return INSTANCE.Create();
        }


        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the <code>java.lang.String</code> representation
         * of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public static Option Create(string opt) {
            return INSTANCE.Create(opt);
        }
    }


    internal class InternalOptionBuilder : IOptionBuilder {

        /** long option */
        internal string longopt;

        /** option description */
        internal string description;

        /** argument name */
        internal string argName;

        /** is required? */
        internal bool required;

        /** the number of arguments */
        internal int numberOfArgs = Option.UNINITIALIZED;

        /** option type */
        internal Type type;

        /** option can have an optional argument value */
        internal bool optionalArg;

        /** value separator for argument value */
        internal char valuesep;


        /**
         * private constructor to prevent instances being created
         */
        internal InternalOptionBuilder() {
            // hide the constructor
        }


        /**
         * Resets the member variables to their default values.
         */
        internal void Reset() {
            description = null;
            argName = null;
            longopt = null;
            type = typeof(string);
            required = false;
            numberOfArgs = Option.UNINITIALIZED;
            optionalArg = false;
            valuesep = (char)0;
        }


        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder WithLongOpt(string newLongopt) {
            this.longopt = newLongopt;

            return this;
        }


        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasArg() {
            this.numberOfArgs = 1;

            return this;
        }


        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasArg(bool hasArg) {
            this.numberOfArgs = hasArg ? 1 : Option.UNINITIALIZED;

            return this;
        }


        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder WithArgName(string name) {
            this.argName = name;

            return this;
        }


        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder IsRequired() {
            this.required = true;

            return this;
        }


        /**
         * The next Option created uses <code>sep</code> as a means to
         * separate argument values.
         * <p>
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator('=')
         *                           .create('D');
         *
         * String args = "-Dkey=value";
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);  // will be "key"
         * String propertyValue = opt.getValue(1); // will be "value"
         * </pre>
         *
         * @param sep The value separator to be used for the argument values.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder WithValueSeparator(char sep) {
            this.valuesep = sep;

            return this;
        }


        /**
         * The next Option created uses '<code>=</code>' as a means to
         * separate argument values.
         *
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator()
         *                           .create('D');
         *
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);
         * String propertyValue = opt.getValue(1);
         * </pre>
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder WithValueSeparator() {
            this.valuesep = '=';

            return this;
        }


        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        public IOptionBuilder IsRequired(bool newRequired) {
            this.required = newRequired;

            return this;
        }


        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasArgs() {
            this.numberOfArgs = Option.UNLIMITED_VALUES;

            return this;
        }


        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasArgs(int num) {
            this.numberOfArgs = num;

            return this;
        }


        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasOptionalArg() {
            this.numberOfArgs = 1;
            this.optionalArg = true;

            return this;
        }


        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasOptionalArgs() {
            this.numberOfArgs = Option.UNLIMITED_VALUES;
            this.optionalArg = true;

            return this;
        }


        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        public IOptionBuilder HasOptionalArgs(int numArgs) {
            this.numberOfArgs = numArgs;
            this.optionalArg = true;

            return this;
        }


        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         * <p>
         * <b>Note:</b> this method is kept for binary compatibility and the
         * input type is supposed to be a {@link Class} object. 
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @deprecated since 1.3, use {@link #withType(Class)} instead
         */
        [Obsolete]
        public IOptionBuilder WithOptionType(object newType) {
            return WithOptionType((Type)newType);
        }


        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        public IOptionBuilder WithOptionType(Type newType) {
            this.type = newType;

            return this;
        }


        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        public IOptionBuilder WithDescription(string newDescription) {
            this.description = newDescription;

            return this;
        }


        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public Option Create(char opt) {
            return Create(Convert.ToString(opt));
        }


        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        public Option Create() {
            if (longopt == null) {
                Reset();
                throw new ArgumentException("must specify longopt");
            }

            return Create(null);
        }


        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the <code>java.lang.String</code> representation
         * of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public Option Create(string opt) {
            Option option = null;
            
            try {
                // create the option
                option = new Option(opt, description);

                // set the option properties
                option.SetLongOpt(longopt);
                option.SetRequired(required);
                option.SetOptionalArg(optionalArg);
                option.SetArgs(numberOfArgs);
                option.SetOptionType(type);
                option.SetValueSeparator(valuesep);
                option.SetArgName(argName);
            }
            finally {
                // reset the OptionBuilder properties
                Reset();
            }

            // return the Option instance
            return option;
        }
    }
}
