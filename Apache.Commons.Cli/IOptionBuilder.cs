
namespace Apache.Commons.Cli {

    using System;

    [Obsolete]
    public interface IOptionBuilder {

        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        IOptionBuilder WithLongOpt(string newLongopt);


        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasArg();


        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasArg(bool hasArg);


        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        IOptionBuilder WithArgName(string name);


        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder IsRequired();


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
        IOptionBuilder WithValueSeparator(char sep);


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
        IOptionBuilder WithValueSeparator();


        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        IOptionBuilder IsRequired(bool newRequired);


        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasArgs();


        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasArgs(int num);


        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasOptionalArg();


        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasOptionalArgs();


        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        IOptionBuilder HasOptionalArgs(int numArgs);


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
        IOptionBuilder WithOptionType(object newType);


        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        IOptionBuilder WithOptionType(Type newType);


        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        IOptionBuilder WithDescription(string newDescription);


        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        Option Create(char opt);


        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        Option Create();


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
        Option Create(string opt);
    }
}
