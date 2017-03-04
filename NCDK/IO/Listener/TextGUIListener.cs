/* Copyright (C) 2003-2007  The Jmol Development Team
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.IO.Setting;
using System;
using System.IO;
using static NCDK.IO.Setting.IOSetting;

namespace NCDK.IO.Listener
{
    /// <summary>
    /// Allows processing of IOSetting quesions which are passed to the user
    /// by using the System.out and System.in by default.
    ///
    /// <p>This listener can also be used to list all the questions a ChemObjectWriter
    /// has, by using a dummy StringWriter, and a <code>null</code> Reader.
    ///
    // @cdk.module io
    // @cdk.githash
    ///
    // @author Egon Willighagen <egonw@sci.kun.nl>
    /// </summary>
    public class TextGUIListener : IReaderListener, IWriterListener
    {
        private TextReader in_;
        private TextWriter out_;

        private Importance level = Importance.High;

        public TextGUIListener(Importance level)
        {
            this.level = level;
            this.SetInputReader(System.Console.In);
            this.SetOutputWriter(System.Console.Out);
        }

        public void SetLevel(Importance level)
        {
            this.level = level;
        }

        /// <summary>
        /// Overwrites the default writer to which the output is directed.
        /// </summary>
        public void SetOutputWriter(TextWriter writer)
        {
            out_ = writer;
        }

        /// <summary>
        /// Overwrites the default reader from which the input is taken.
        /// </summary>
        public void SetInputReader(TextReader reader)
        {
            in_ = reader;
        }

        public void FrameRead(ReaderEvent evt) { }

        /// <summary>
        /// Processes the IOSettings by listing the question, giving the options
        /// and asking the user to provide their choice.
        ///
        /// <p>Note: if the input reader is <code>null</code>, then the method
        /// does not wait for an answer, and takes the default.
        /// </summary>
        public void ProcessIOSettingQuestion(IOSetting setting)
        {
            // post the question
            if (setting.Level.Ordinal <= this.level.Ordinal)
            {
                // output the option name
                this.out_.Write("[" + setting.Name + "]: ");
                // post the question
                this.out_.Write(setting.Question);
                if (setting is BooleanIOSetting)
                {
                    BooleanIOSetting boolSet = (BooleanIOSetting)setting;
                    bool set = boolSet.IsSet;
                    if (set)
                    {
                        this.out_.Write(" [Yn]");
                    }
                    else
                    {
                        this.out_.Write(" [yN]");
                    }
                }
                else if (setting is OptionIOSetting)
                {
                    OptionIOSetting optionSet = (OptionIOSetting)setting;
                    var settings = optionSet.GetOptions();
                    for (int i = 0; i < settings.Count; i++)
                    {
                        this.out_.WriteLine();
                        string option = (string)settings[i];
                        this.out_.Write((i + 1) + ". " + option);
                        if (option.Equals(setting.Setting))
                        {
                            this.out_.Write(" (Default)");
                        }
                    }
                }
                else
                {
                    this.out_.Write(" [" + setting.Setting + "]");
                }
                this.out_.WriteLine();
                this.out_.Flush();

                // get the answer, only if input != null
                if (this.in_ == null)
                {
                    // don't really ask questions. This is intentional behaviour to
                    // allow for listing all questions. The settings is now defaulted,
                    // which is the intention too.
                }
                else
                {
                    bool gotAnswer = false;
                    while (!gotAnswer)
                    {
                        try
                        {
                            this.out_.Write("> ");
                            this.out_.Flush();
                            string answer = in_.ReadLine();
                            if (answer.Length == 0)
                            {
                                // pressed ENTER -> take default
                            }
                            else if (setting is OptionIOSetting)
                            {
                                ((OptionIOSetting)setting).SetSetting(int.Parse(answer));
                            }
                            else if (setting is BooleanIOSetting)
                            {
                                switch (answer.ToLowerInvariant())
                                {
                                    case "n":
                                    case "no":
                                        answer = "false";
                                        break;
                                    case "y":
                                    case "yes":
                                        answer = "true";
                                        break;
                                }
                                setting.Setting = answer;
                            }
                            else
                            {
                                setting.Setting = answer;
                            }
                            gotAnswer = true;
                        }
                        catch (IOException)
                        {
                            this.out_.WriteLine("Cannot read from STDIN. Skipping question.");
                        }
                        catch (FormatException)
                        {
                            this.out_.WriteLine("Answer is not a number.");
                        }
                        catch (CDKException exception)
                        {
                            this.out_.WriteLine();
                            this.out_.WriteLine(exception.ToString());
                        }
                    }
                }
            }
        }
    }
}
