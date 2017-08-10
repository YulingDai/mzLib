using MassSpectrometry;
using MzLibUtil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;



namespace IO.MzML
{
    public static class MzmlMethods
    {

        internal static readonly XmlSchemaSet schemaSet = new XmlSchemaSet();
        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "Test")]


        #region Internal Fields

        internal static readonly XmlSerializer indexedSerializer = new XmlSerializer(typeof(Generated.indexedmzML));
        internal static readonly XmlSerializer mzmlSerializer = new XmlSerializer(typeof(Generated.mzMLType));

        #endregion Internal Fields

        #region Private Fields

        private static readonly Dictionary<DissociationType, string> DissociationTypeAccessions = new Dictionary<DissociationType, string>{
            {DissociationType.CID, "MS:1000133"},
            {DissociationType.ISCID, "MS:1001880"},
            {DissociationType.HCD, "MS:1000422" },
            {DissociationType.ETD, "MS:1000598"},
            {DissociationType.MPD, "MS:1000435"},
            {DissociationType.PQD, "MS:1000599"},
            {DissociationType.Unknown, "MS:1000044"} };

        private static readonly Dictionary<DissociationType, string> DissociationTypeNames = new Dictionary<DissociationType, string>{
            {DissociationType.CID, "collision-induced dissociation"},
            {DissociationType.ISCID, "in-source collision-induced dissociation"},
            {DissociationType.HCD, "beam-type collision-induced dissociation"},
            {DissociationType.ETD, "electron transfer dissociation"},
            {DissociationType.MPD, "photodissociation"},
            {DissociationType.PQD, "pulsed q dissociation"},
            {DissociationType.Unknown, "dissociation method"}};

        private static readonly Dictionary<MZAnalyzerType, string> analyzerDictionary = new Dictionary<MZAnalyzerType, string>
            {
                {MZAnalyzerType.Unknown, "MS:1000443"},
                {MZAnalyzerType.Quadrupole, "MS:1000081"},
                {MZAnalyzerType.IonTrap2D, "MS:1000291"},
                {MZAnalyzerType.IonTrap3D,"MS:1000082"},
                {MZAnalyzerType.Orbitrap,"MS:1000484"},
                {MZAnalyzerType.TOF,"MS:1000084"},
                {MZAnalyzerType.FTICR ,"MS:1000079"},
                {MZAnalyzerType.Sector,"MS:1000080"}
            };

        private static readonly Dictionary<bool, string> CentroidAccessions = new Dictionary<bool, string>{
            {true, "MS:1000127"},
            {false, "MS:1000128"}};

        private static readonly Dictionary<bool, string> CentroidNames = new Dictionary<bool, string>{
            {true, "centroid spectrum"},
            {false, "profile spectrum"}};

        private static readonly Dictionary<Polarity, string> PolarityAccessions = new Dictionary<Polarity, string>{
            {Polarity.Negative, "MS:1000129"},
            {Polarity.Positive, "MS:1000130"}};

        private static readonly Dictionary<Polarity, string> PolarityNames = new Dictionary<Polarity, string>{
            {Polarity.Negative, "negative scan"},
            {Polarity.Positive, "positive scan"}};
        #endregion Private Fields

        #region Public Methods

        public static void CreateAndWriteMyMzmlWithCalibratedSpectra(IMsDataFile<IMsDataScan<IMzSpectrum<IMzPeak>>> myMsDataFile, string outputFile, bool writeIndexed)
        {
            Generated.indexedmzML indexedMzml = new Generated.indexedmzML();
            string title = System.IO.Path.GetFileNameWithoutExtension(outputFile);
            schemaSet.Add("http://psi.hupo.org/ms/mzml", "http://psidev.info/files/ms/mzML/xsd/mzML1.1.2_idx.xsd");
            schemaSet.Add("http://psi.hupo.org/ms/mzml", " http://psidev.cvs.sourceforge.net/viewvc/psidev/psi/psi-ms/mzML/schema/mzML1.1.0.xsd");




            var mzML = new Generated.mzMLType()
            {
                version = "1.1.0",
                cvList = new Generated.CVListType(),
                id = title,
            };


            mzML.cvList = new Generated.CVListType()
            {
                count = "2",
                cv = new Generated.CVType[2]

            };
            mzML.cvList.cv[0] = new Generated.CVType()
            {
                URI = @"https://raw.githubusercontent.com/HUPO-PSI/psi-ms-CV/master/psi-ms.obo",
                fullName = "Proteomics Standards Initiative Mass Spectrometry Ontology",
                id = "MS",
                version = "4.0.1"
            };

            mzML.cvList.cv[1] = new Generated.CVType()
            {
                URI = @"http://obo.cvs.sourceforge.net/*checkout*/obo/obo/ontology/phenotype/unit.obo",
                fullName = "Unit Ontology",
                id = "UO",
                version = "12:10:2011"
            };
            mzML.fileDescription = new Generated.FileDescriptionType()
            {
                fileContent = new Generated.ParamGroupType(),
                sourceFileList = new Generated.SourceFileListType()

            };

            #region MSGF
            //This is info required only by MS-GF+ for some reason...



            mzML.fileDescription.sourceFileList = new Generated.SourceFileListType()
            {
                count = "2",
                sourceFile = new Generated.SourceFileType[2]
            };



            mzML.fileDescription.sourceFileList.sourceFile[0] = new Generated.SourceFileType()
            {
                id = "SF",
                name = "example.raw",
                location = @"C:\Users",
                cvParam = new Generated.CVParamType[3],
            };


            mzML.fileDescription.sourceFileList.sourceFile[0].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000776", //scan only ID type
                name = "scan number only nativeID format",
                cvRef = "MS",
                value = ""
            };

            mzML.fileDescription.sourceFileList.sourceFile[0].cvParam[1] = new Generated.CVParamType()
            {
                accession = "MS:1000563",
                name = "Thermo RAW format",
                value = "",
                cvRef = "MS",


            };

            mzML.fileDescription.sourceFileList.sourceFile[0].cvParam[2] = new Generated.CVParamType()
            {
                cvRef = "MS",
                accession = "MS:1000569",
                name = "SHA-1",
                value = "b43e9286b40e8b5dbc0dfa2e428495769ca96a96"
            };

            mzML.fileDescription.sourceFileList.sourceFile[1] = new Generated.SourceFileType()
            {
                id = title + ".mzML",
                name = title + ".mzML",
                location = @"file:///C:\Users\",
                cvParam = new Generated.CVParamType[1]
            };

            mzML.fileDescription.sourceFileList.sourceFile[1].cvParam[0] = new Generated.CVParamType()
            {
                cvRef = "MS",
                accession = "MS:1000569",
                name = "SHA-1",
                value = "1cb2a01819513c32d31fcd44aa77ae076ac6a48b"
            };


            #endregion MSGF

            mzML.fileDescription.fileContent.cvParam = new Generated.CVParamType[2];
            mzML.fileDescription.fileContent.cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000579", // MS1 Data
                name = "MS1 spectrum",
                cvRef = "MS",
                value = ""
            };
            mzML.fileDescription.fileContent.cvParam[1] = new Generated.CVParamType()
            {
                accession = "MS:1000580", // MSn Data   
                name = "MSn spectrum",
                cvRef = "MS",
                value = ""
            };
            mzML.softwareList = new Generated.SoftwareListType()
            {
                count = "2",
                software = new Generated.SoftwareType[2]
            };

            // TODO: add the raw file fields
            mzML.softwareList.software[0] = new Generated.SoftwareType()
            {
                id = "mzLib",
                version = "1",
                cvParam = new Generated.CVParamType[1]
            };

            mzML.softwareList.software[1] = new Generated.SoftwareType()
            {
                id = "pwiz_3.0.9967",
                version = "3.0.9967",
                cvParam = new Generated.CVParamType[1]
            };

            mzML.softwareList.software[1].cvParam[0] = new Generated.CVParamType()
            {
                cvRef = "MS",
                accession = "MS:1000615",
                name = "ProteoWizard software",
                value = ""
            };

            mzML.softwareList.software[0].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000799",
                value = "mzLib",
                name = "custom unreleased software tool",
                cvRef = "MS"
            };

            List<MZAnalyzerType> analyzersInThisFile = (new HashSet<MZAnalyzerType>(myMsDataFile.Select(b => b.MzAnalyzer))).ToList();
            Dictionary<MZAnalyzerType, string> analyzersInThisFileDict = new Dictionary<MZAnalyzerType, string>();

            // Leaving empty. Can't figure out the configurations.
            // ToDo: read instrumentConfigurationList from mzML file
            mzML.instrumentConfigurationList = new Generated.InstrumentConfigurationListType()
            {
                count = analyzersInThisFile.Count.ToString(),
                instrumentConfiguration = new Generated.InstrumentConfigurationType[analyzersInThisFile.Count]
            };

            // Write the analyzers, also the default, also in the scans if needed

            for (int i = 0; i < mzML.instrumentConfigurationList.instrumentConfiguration.Length; i++)
            {
                analyzersInThisFileDict[analyzersInThisFile[i]] = "IC" + (i + 1).ToString();
                mzML.instrumentConfigurationList.instrumentConfiguration[i] = new Generated.InstrumentConfigurationType()
                {
                    id = "IC" + (i + 1).ToString(),
                    componentList = new Generated.ComponentListType(),
                    cvParam = new Generated.CVParamType[1]
                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = "MS:1000031",
                    name = "instrument model",
                    value = ""

                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList = new Generated.ComponentListType()
                {
                    count = 3.ToString(),
                    source = new Generated.SourceComponentType[1],
                    analyzer = new Generated.AnalyzerComponentType[1],
                    detector = new Generated.DetectorComponentType[1],
                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.source[0] = new Generated.SourceComponentType()
                {
                    order = 1,
                    cvParam = new Generated.CVParamType[1]
                };
                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.source[0].cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = "MS:1000073",
                    name = "electrospray ionization",
                    value = ""
                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.analyzer[0] = new Generated.AnalyzerComponentType()
                {
                    order = i + 2,
                    cvParam = new Generated.CVParamType[1]
                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.analyzer[0].cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = analyzerDictionary[analyzersInThisFile[i]],
                    name = analyzersInThisFile[i].ToString().ToLower(),
                    value = ""
                };

                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.detector[0] = new Generated.DetectorComponentType()
                {
                    order = 3,
                    cvParam = new Generated.CVParamType[1]
                };
                mzML.instrumentConfigurationList.instrumentConfiguration[i].componentList.detector[0].cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = "MS:1000026",
                    name = "detector type",
                    value = ""
                };

            }

            mzML.dataProcessingList = new Generated.DataProcessingListType()
            {
                count = "2",
                dataProcessing = new Generated.DataProcessingType[2]
            };

            // Only writing mine! Might have had some other data processing (but not if it is a raw file)
            // ToDo: read dataProcessingList from mzML file
            mzML.dataProcessingList.dataProcessing[0] = new Generated.DataProcessingType()
            {
                id = "mzLibProcessing",
                processingMethod = new Generated.ProcessingMethodType[1],
            };

            mzML.dataProcessingList.dataProcessing[1] = new Generated.DataProcessingType()
            {
                id = "pwiz_Reader_conversion",
                processingMethod = new Generated.ProcessingMethodType[1],
            };
            mzML.dataProcessingList.dataProcessing[0].processingMethod[0] = new Generated.ProcessingMethodType()
            {
                order = "0",
                softwareRef = "mzLib",
                cvParam = new Generated.CVParamType[1],

            };

            mzML.dataProcessingList.dataProcessing[1].processingMethod[0] = new Generated.ProcessingMethodType()
            {
                order = "0",
                softwareRef = "pwiz_3.0.9967",
                cvParam = new Generated.CVParamType[1],

            };

            mzML.dataProcessingList.dataProcessing[1].processingMethod[0].cvParam[0] = new Generated.CVParamType()
            {
                cvRef = "MS",
                accession = "MS:1000544",
                name = "Conversion to mzML",
                value = ""

            };
            mzML.dataProcessingList.dataProcessing[0].processingMethod[0].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000544",
                cvRef = "MS",
                name = "Conversion to mzML",
                value = ""
            };
            mzML.run = new Generated.RunType()
            {
                defaultInstrumentConfigurationRef = analyzersInThisFileDict[analyzersInThisFile[0]],
                id = title
            };

            mzML.run.chromatogramList = new Generated.ChromatogramListType()
            {
                count = "1",
                chromatogram = new Generated.ChromatogramType[1],
                defaultDataProcessingRef = "pwiz_Reader_conversion"
            };
            // ToDo: Finish the chromatogram writing! (think finished)

            //Chromatagram info
            mzML.run.chromatogramList.chromatogram[0] = new Generated.ChromatogramType()
            {
                defaultArrayLength = myMsDataFile.NumSpectra,
                id = "TIC",
                index = "0",
                dataProcessingRef = "mzLibProcessing",
                binaryDataArrayList = new Generated.BinaryDataArrayListType()
                {
                    count = "2",
                    binaryDataArray = new Generated.BinaryDataArrayType[2]

                },
                cvParam = new Generated.CVParamType[1]
            };

            mzML.run.chromatogramList.chromatogram[0].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000235",
                name = "total ion current chromatogram",
                cvRef = "MS",
                value = ""
            };


            double[] times = new double[myMsDataFile.NumSpectra];
            double[] intensities = new double[myMsDataFile.NumSpectra];

            for (int i = 1; i <= myMsDataFile.NumSpectra; i++)
            {
                times[i - 1] = myMsDataFile.GetOneBasedScan(i).RetentionTime;
                intensities[i - 1] = myMsDataFile.GetOneBasedScan(i).MassSpectrum.SumOfAllY;
            }

            //Chromatofram X axis (time)
            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0] = new Generated.BinaryDataArrayType()
            {
                binary = MzSpectrum<MzPeak>.Get64Bitarray(times)
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].encodedLength = (4 * Math.Ceiling(((double)mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);

            //mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam = new Generated.CVParamType[3];
            //mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam[0] = new Generated.CVParamType();

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam = new Generated.CVParamType[3];

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000523",
                name = "64-bit float",
                cvRef = "MS",
                value = ""
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam[1] = new Generated.CVParamType()
            {
                accession = "MS:1000576",
                name = "no compression",
                cvRef = "MS",
                value = ""
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[0].cvParam[2] = new Generated.CVParamType()
            {
                accession = "MS:1000595",
                name = "time array",
                unitCvRef = "UO",
                unitAccession = "UO:0000031",
                unitName = "Minutes",
                cvRef = "MS",
                value = ""
            };

            //Chromatogram Y axis (total intensity)
            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1] = new Generated.BinaryDataArrayType()
            {
                binary = MzSpectrum<MzPeak>.Get64Bitarray(intensities)
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].encodedLength = (4 * Math.Ceiling(((double)mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);


            //mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].dataProcessingRef = "mzLibProcessing";

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].cvParam = new Generated.CVParamType[3];

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].cvParam[0] = new Generated.CVParamType()
            {
                accession = "MS:1000523",
                name = "64-bit float",
                cvRef = "MS",
                value = ""
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].cvParam[1] = new Generated.CVParamType()
            {
                accession = "MS:1000576",
                name = "no compression",
                cvRef = "MS",
                value = ""
            };

            mzML.run.chromatogramList.chromatogram[0].binaryDataArrayList.binaryDataArray[1].cvParam[2] = new Generated.CVParamType()
            {
                accession = "MS:1000515",
                name = "intensity array",
                unitCvRef = "MS",
                unitAccession = "MS:1000131",
                unitName = "number of counts",
                cvRef = "MS",
                value = "",
            };

            mzML.run.spectrumList = new Generated.SpectrumListType()
            {
                count = (myMsDataFile.NumSpectra).ToString(CultureInfo.InvariantCulture),
                defaultDataProcessingRef = "pwiz_Reader_conversion",
                spectrum = new Generated.SpectrumType[myMsDataFile.NumSpectra]
            };

            // Loop over all spectra
            for (int i = 1; i <= myMsDataFile.NumSpectra; i++)
            {
                mzML.run.spectrumList.spectrum[i - 1] = new Generated.SpectrumType()
                {
                    defaultArrayLength = myMsDataFile.GetOneBasedScan(i).MassSpectrum.YArray.Length,
                    index = (i - 1).ToString(CultureInfo.InvariantCulture),
                    id = "scan=" + (myMsDataFile.GetOneBasedScan(i).OneBasedScanNumber).ToString(),
                    //"controllerType=0 controllerNumber=1 
                    cvParam = new Generated.CVParamType[10],
                    scanList = new Generated.ScanListType()
                };

                mzML.run.spectrumList.spectrum[i - 1].scanList = new Generated.ScanListType()
                {
                    count = 1.ToString(),
                    scan = new Generated.ScanType[1],
                    cvParam = new Generated.CVParamType[1]
                };
                mzML.run.spectrumList.spectrum[i - 1].scanList.cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = "MS:1000795",
                    name = "no combination",
                    value = ""
                };
                var h = myMsDataFile.GetOneBasedScan(i).MzAnalyzer;
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0] = new Generated.ScanType
                {
                };



                if (myMsDataFile.GetOneBasedScan(i).MsnOrder == 1)
                {
                    mzML.run.spectrumList.spectrum[i - 1].cvParam[0] = new Generated.CVParamType()
                    {
                        accession = "MS:1000579",
                        cvRef = "MS",
                        name = "MS1 spectrum",
                        value = ""
                    };
                }
                else if (myMsDataFile.GetOneBasedScan(i) is IMsDataScanWithPrecursor<IMzSpectrum<IMzPeak>>)
                {
                    var scanWithPrecursor = myMsDataFile.GetOneBasedScan(i) as IMsDataScanWithPrecursor<IMzSpectrum<IMzPeak>>;
                    mzML.run.spectrumList.spectrum[i - 1].cvParam[0] = new Generated.CVParamType
                    {
                        accession = "MS:1000580",
                        cvRef = "MS",
                        name = "MSn spectrum",
                        value = ""
                    };
                    string precursorID = "scan=" + scanWithPrecursor.OneBasedPrecursorScanNumber.ToString();

                    // So needs a precursor!
                    mzML.run.spectrumList.spectrum[i - 1].precursorList = new Generated.PrecursorListType()
                    {
                        count = 1.ToString(),
                        precursor = new Generated.PrecursorType[1],
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0] = new Generated.PrecursorType();

                    //note: precursod "id" set to string ID of spectrum (not index)
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].spectrumRef = precursorID;
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].selectedIonList = new Generated.SelectedIonListType()
                    {
                        count = 1.ToString(),
                        selectedIon = new Generated.ParamGroupType[1]
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].selectedIonList.selectedIon[0] = new Generated.ParamGroupType()
                    {
                        cvParam = new Generated.CVParamType[3]
                    };

                    // Selected ion MZ
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[0] = new Generated.CVParamType()
                    {
                        name = "selected ion m/z",
                        value = scanWithPrecursor.SelectedIonMZ.ToString(CultureInfo.InvariantCulture),
                        accession = "MS:1000744",
                        cvRef = "MS",
                        unitCvRef = "MS",
                        unitAccession = "MS:1000040",
                        unitName = "m/z"
                    };

                    // Charge State
                    if (scanWithPrecursor.SelectedIonChargeStateGuess.HasValue)
                    {
                        mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[1] = new Generated.CVParamType()
                        {
                            name = "charge state",
                            value = scanWithPrecursor.SelectedIonChargeStateGuess.Value.ToString(CultureInfo.InvariantCulture),
                            accession = "MS:1000041",
                            cvRef = "MS",

                        };
                    }

                    // Selected ion intensity
                    if (scanWithPrecursor.SelectedIonIntensity.HasValue)
                    {
                        mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].selectedIonList.selectedIon[0].cvParam[2] = new Generated.CVParamType()
                        {
                            name = "peak intensity",
                            value = scanWithPrecursor.SelectedIonIntensity.Value.ToString(CultureInfo.InvariantCulture),
                            accession = "MS:1000042",
                            cvRef = "MS"
                        };
                    }

                    MzRange isolationRange = scanWithPrecursor.IsolationRange;
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].isolationWindow = new Generated.ParamGroupType()
                    {
                        cvParam = new Generated.CVParamType[3]
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].isolationWindow.cvParam[0] = new Generated.CVParamType()
                    {
                        accession = "MS:1000827",
                        name = "isolation window target m/z",
                        value = isolationRange.Mean.ToString(CultureInfo.InvariantCulture),
                        cvRef = "MS",
                        unitCvRef = "MS",
                        unitAccession = "MS:1000040",
                        unitName = "m/z"
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].isolationWindow.cvParam[1] = new Generated.CVParamType()
                    {
                        accession = "MS:1000828",
                        name = "isolation window lower offset",
                        value = (isolationRange.Width / 2).ToString(CultureInfo.InvariantCulture),
                        cvRef = "MS",
                        unitCvRef = "MS",
                        unitAccession = "MS:1000040",
                        unitName = "m/z"
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].isolationWindow.cvParam[2] = new Generated.CVParamType()
                    {
                        accession = "MS:1000829",
                        name = "isolation window upper offset",
                        value = (isolationRange.Width / 2).ToString(CultureInfo.InvariantCulture),
                        cvRef = "MS",
                        unitCvRef = "MS",
                        unitAccession = "MS:1000040",
                        unitName = "m/z"
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation = new Generated.ParamGroupType()
                    {
                        cvParam = new Generated.CVParamType[1]
                    };
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation.cvParam[0] = new Generated.CVParamType();

                    DissociationType dissociationType = scanWithPrecursor.DissociationType;

                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation.cvParam[0].accession = DissociationTypeAccessions[dissociationType];
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation.cvParam[0].name = DissociationTypeNames[dissociationType];
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation.cvParam[0].cvRef = "MS";
                    mzML.run.spectrumList.spectrum[i - 1].precursorList.precursor[0].activation.cvParam[0].value = "";

                }

                mzML.run.spectrumList.spectrum[i - 1].cvParam[1] = new Generated.CVParamType()
                {
                    name = "ms level",
                    accession = "MS:1000511",
                    value = myMsDataFile.GetOneBasedScan(i).MsnOrder.ToString(CultureInfo.InvariantCulture),
                    cvRef = "MS"
                };
                mzML.run.spectrumList.spectrum[i - 1].cvParam[2] = new Generated.CVParamType()
                {
                    name = CentroidNames[myMsDataFile.GetOneBasedScan(i).IsCentroid],
                    accession = CentroidAccessions[myMsDataFile.GetOneBasedScan(i).IsCentroid],
                    cvRef = "MS",
                    value = ""
                };
                if (PolarityNames.TryGetValue(myMsDataFile.GetOneBasedScan(i).Polarity, out string polarityName) && PolarityAccessions.TryGetValue(myMsDataFile.GetOneBasedScan(i).Polarity, out string polarityAccession))
                {
                    mzML.run.spectrumList.spectrum[i - 1].cvParam[3] = new Generated.CVParamType()
                    {
                        name = polarityName,
                        accession = polarityAccession,
                        cvRef = "MS",
                        value = ""
                    };
                }
                // Spectrum title
                //string title = System.IO.Path.GetFileNameWithoutExtension(outputFile);
                mzML.run.spectrumList.spectrum[i - 1].cvParam[4] = new Generated.CVParamType()
                {
                    name = "spectrum title",
                    accession = "MS:1000796",
                    value = title + "." + (i + 1) + "." + (i + 1) + "." + " File:\"\", NativeID:\"" + mzML.run.spectrumList.spectrum[i - 1].id + "\"",
                    cvRef = "MS",

                };
                if ((myMsDataFile.GetOneBasedScan(i).MassSpectrum.Size) > 0)
                {
                    // Lowest observed mz
                    mzML.run.spectrumList.spectrum[i - 1].cvParam[5] = new Generated.CVParamType()
                    {
                        name = "lowest observed m/z",
                        accession = "MS:1000528",
                        value = myMsDataFile.GetOneBasedScan(i).MassSpectrum.FirstX.ToString(CultureInfo.InvariantCulture),
                        unitCvRef = "MS",
                        unitAccession = "MS:1000040",
                        unitName = "m/z",
                        cvRef = "MS"
                    };

                    // Highest observed mz
                    mzML.run.spectrumList.spectrum[i - 1].cvParam[6] = new Generated.CVParamType()
                    {
                        name = "highest observed m/z",
                        accession = "MS:1000527",
                        value = myMsDataFile.GetOneBasedScan(i).MassSpectrum.LastX.ToString(CultureInfo.InvariantCulture),
                        unitAccession = "MS:1000040",
                        unitName = "m/z",
                        cvRef = "MS"
                    };
                }

                // Total ion current
                mzML.run.spectrumList.spectrum[i - 1].cvParam[7] = new Generated.CVParamType()
                {
                    name = "total ion current",
                    accession = "MS:1000285",
                    value = myMsDataFile.GetOneBasedScan(i).TotalIonCurrent.ToString(CultureInfo.InvariantCulture),
                    cvRef = "MS",
                };

                //base peak m/z
                mzML.run.spectrumList.spectrum[i - 1].cvParam[8] = new Generated.CVParamType()
                {
                    name = "base peak m/z",
                    accession = "MS:1000504",
                    value = myMsDataFile.GetOneBasedScan(i).MassSpectrum.PeakWithHighestY.Mz.ToString(),
                    unitCvRef = "MS",
                    unitName = "m/z",
                    unitAccession = "MS:1000040",
                    cvRef = "MS"
                };

                //base peak intensity
                mzML.run.spectrumList.spectrum[i - 1].cvParam[9] = new Generated.CVParamType()
                {
                    name = "base peak intensity",
                    accession = "MS:1000505",
                    value = myMsDataFile.GetOneBasedScan(i).MassSpectrum.YofPeakWithHighestY.ToString(),
                    unitCvRef = "MS",
                    unitName = "number of detector",
                    unitAccession = "MS:1000131",
                    cvRef = "MS"
                };

                // Retention time
                mzML.run.spectrumList.spectrum[i - 1].scanList = new Generated.ScanListType()
                {
                    count = "1",
                    scan = new Generated.ScanType[1],
                    cvParam = new Generated.CVParamType[1]
                };

                mzML.run.spectrumList.spectrum[i - 1].scanList.cvParam[0] = new Generated.CVParamType()
                {
                    cvRef = "MS",
                    accession = "MS:1000795",
                    name = "no combination",
                    value = ""
                };

                if (myMsDataFile.GetOneBasedScan(i).MzAnalyzer.Equals(analyzersInThisFile[0]))
                {
                    mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0] = new Generated.ScanType()
                    {
                        cvParam = new Generated.CVParamType[3]
                    };
                }
                else
                {
                    mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0] = new Generated.ScanType()
                    {
                        cvParam = new Generated.CVParamType[3],
                        instrumentConfigurationRef = analyzersInThisFileDict[myMsDataFile.GetOneBasedScan(i).MzAnalyzer]
                    };
                }
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].cvParam[0] = new Generated.CVParamType()
                {
                    name = "scan start time",
                    accession = "MS:1000016",
                    value = myMsDataFile.GetOneBasedScan(i).RetentionTime.ToString(CultureInfo.InvariantCulture),
                    unitCvRef = "UO",
                    unitAccession = "UO:0000031",
                    unitName = "minute",
                    cvRef = "MS",


                };
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].cvParam[1] = new Generated.CVParamType()
                {
                    name = "filter string",
                    accession = "MS:1000512",
                    value = myMsDataFile.GetOneBasedScan(i).ScanFilter,
                    cvRef = "MS"
                };
                if (myMsDataFile.GetOneBasedScan(i).InjectionTime.HasValue)
                {
                    mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].cvParam[2] = new Generated.CVParamType()
                    {
                        name = "ion injection time",
                        accession = "MS:1000927",
                        value = myMsDataFile.GetOneBasedScan(i).InjectionTime.Value.ToString(CultureInfo.InvariantCulture),
                        cvRef = "MS",
                        unitName = "millisecond",
                        unitAccession = "UO:0000028",
                        unitCvRef = "UO"
                    };
                }
                if (myMsDataFile.GetOneBasedScan(i) is IMsDataScanWithPrecursor<IMzSpectrum<IMzPeak>>)
                {
                    var scanWithPrecursor = myMsDataFile.GetOneBasedScan(i) as IMsDataScanWithPrecursor<IMzSpectrum<IMzPeak>>;
                    if (scanWithPrecursor.SelectedIonMonoisotopicGuessMz.HasValue)
                    {
                        mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].userParam = new Generated.UserParamType[1];
                        mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].userParam[0] = new Generated.UserParamType()
                        {
                            name = "[mzLib]Monoisotopic M/Z:",
                            value = scanWithPrecursor.SelectedIonMonoisotopicGuessMz.Value.ToString(CultureInfo.InvariantCulture)
                        };
                    }
                }

                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].scanWindowList = new Generated.ScanWindowListType()
                {
                    count = 1,
                    scanWindow = new Generated.ParamGroupType[1]
                };
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].scanWindowList.scanWindow[0] = new Generated.ParamGroupType()
                {
                    cvParam = new Generated.CVParamType[2]
                };
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].scanWindowList.scanWindow[0].cvParam[0] = new Generated.CVParamType()
                {
                    name = "scan window lower limit",
                    accession = "MS:1000501",
                    value = myMsDataFile.GetOneBasedScan(i).ScanWindowRange.Minimum.ToString(CultureInfo.InvariantCulture),
                    cvRef = "MS",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000040",
                    unitName = "m/z"
                };
                mzML.run.spectrumList.spectrum[i - 1].scanList.scan[0].scanWindowList.scanWindow[0].cvParam[1] = new Generated.CVParamType()
                {
                    name = "scan window upper limit",
                    accession = "MS:1000500",
                    value = myMsDataFile.GetOneBasedScan(i).ScanWindowRange.Maximum.ToString(CultureInfo.InvariantCulture),
                    cvRef = "MS",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000040",
                    unitName = "m/z"
                };
                if (myMsDataFile.GetOneBasedScan(i).NoiseData == null)
                {
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList = new Generated.BinaryDataArrayListType()
                    {
                        // ONLY WRITING M/Z AND INTENSITY DATA, NOT THE CHARGE! (but can add charge info later)
                        // CHARGE (and other stuff) CAN BE IMPORTANT IN ML APPLICATIONS!!!!!
                        count = 2.ToString(),
                        binaryDataArray = new Generated.BinaryDataArrayType[2]
                    };
                }

                if (myMsDataFile.GetOneBasedScan(i).NoiseData != null)
                {
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList = new Generated.BinaryDataArrayListType()
                    {
                        // ONLY WRITING M/Z AND INTENSITY DATA, NOT THE CHARGE! (but can add charge info later)
                        // CHARGE (and other stuff) CAN BE IMPORTANT IN ML APPLICATIONS!!!!!
                        count = 5.ToString(),
                        binaryDataArray = new Generated.BinaryDataArrayType[5]
                    };
                }

                // M/Z Data
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0] = new Generated.BinaryDataArrayType()
                {
                    binary = myMsDataFile.GetOneBasedScan(i).MassSpectrum.Get64BitXarray()
                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].encodedLength = (4 * Math.Ceiling(((double)mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].cvParam = new Generated.CVParamType[3];
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].cvParam[0] = new Generated.CVParamType()
                {
                    accession = "MS:1000514",
                    name = "m/z array",
                    cvRef = "MS",
                    unitName = "m/z",
                    value = "",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000040",
                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].cvParam[1] = new Generated.CVParamType()
                {
                    accession = "MS:1000523",
                    name = "64-bit float",
                    cvRef = "MS",
                    value = ""

                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[0].cvParam[2] = new Generated.CVParamType()
                {
                    accession = "MS:1000576",
                    name = "no compression",
                    cvRef = "MS",
                    value = ""
                };

                // Intensity Data
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1] = new Generated.BinaryDataArrayType()
                {
                    binary = myMsDataFile.GetOneBasedScan(i).MassSpectrum.Get64BitYarray()
                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].encodedLength = (4 * Math.Ceiling(((double)mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].cvParam = new Generated.CVParamType[3];
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].cvParam[0] = new Generated.CVParamType()
                {
                    accession = "MS:1000515",
                    name = "intensity array",
                    cvRef = "MS",
                    unitCvRef = "MS",
                    unitAccession = "MS:1000131",
                    unitName = "number of counts",
                    value = ""
                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].cvParam[1] = new Generated.CVParamType()
                {
                    accession = "MS:1000523",
                    name = "64-bit float",
                    cvRef = "MS",
                    value = ""
                };
                mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[1].cvParam[2] = new Generated.CVParamType()
                {
                    accession = "MS:1000576",
                    name = "no compression",
                    cvRef = "MS",
                    value = ""
                };

                if (myMsDataFile.GetOneBasedScan(i).NoiseData != null)
                {
                    // mass
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2] = new Generated.BinaryDataArrayType()
                    {
                        binary = myMsDataFile.GetOneBasedScan(i).Get64BitNoiseDataMass()
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].arrayLength = (mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].binary.Length / 8).ToString();
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].encodedLength = (4 * Math.Ceiling(((double)mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].cvParam = new Generated.CVParamType[3];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].cvParam[0] = new Generated.CVParamType()
                    {
                        accession = "MS:1000786",
                        name = "non-standard data array",
                        cvRef = "MS",
                        value = "mass",
                        unitCvRef = "MS",

                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].cvParam[1] = new Generated.CVParamType()
                    {
                        accession = "MS:1000523",
                        name = "64-bit float",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].cvParam[2] = new Generated.CVParamType()
                    {
                        accession = "MS:1000576",
                        name = "no compression",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].userParam = new Generated.UserParamType[1];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[2].userParam[0] = new Generated.UserParamType()
                    {
                        name = "kelleherCustomType",
                        value = "noise m/z",
                    };

                    // noise
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3] = new Generated.BinaryDataArrayType()
                    {
                        binary = myMsDataFile.GetOneBasedScan(i).Get64BitNoiseDataNoise()
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].arrayLength = (mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].binary.Length / 8).ToString();
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].encodedLength = (4 * Math.Ceiling(((double)mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].cvParam = new Generated.CVParamType[3];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].cvParam[0] = new Generated.CVParamType()
                    {
                        accession = "MS:1000786",
                        name = "non-standard data array",
                        cvRef = "MS",
                        value = "SignalToNoise"
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].cvParam[1] = new Generated.CVParamType()
                    {
                        accession = "MS:1000523",
                        name = "64-bit float",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].cvParam[2] = new Generated.CVParamType()
                    {
                        accession = "MS:1000576",
                        name = "no compression",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].userParam = new Generated.UserParamType[1];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[3].userParam[0] = new Generated.UserParamType()
                    {
                        name = "kelleherCustomType",
                        value = "noise baseline",
                    };

                    // baseline
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4] = new Generated.BinaryDataArrayType()
                    {
                        binary = myMsDataFile.GetOneBasedScan(i).Get64BitNoiseDataBaseline(),
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].arrayLength = (mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].binary.Length / 8).ToString();
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].encodedLength = (4 * Math.Ceiling(((double)mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].binary.Length / 3))).ToString(CultureInfo.InvariantCulture);
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].cvParam = new Generated.CVParamType[3];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].cvParam[0] = new Generated.CVParamType()
                    {
                        accession = "MS:1000786",
                        name = "non-standard data array",
                        cvRef = "MS",
                        value = "baseline"
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].cvParam[1] = new Generated.CVParamType()
                    {
                        accession = "MS:1000523",
                        name = "64-bit float",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].cvParam[2] = new Generated.CVParamType()
                    {
                        accession = "MS:1000576",
                        name = "no compression",
                        cvRef = "MS",
                        value = ""
                    };
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].userParam = new Generated.UserParamType[1];
                    mzML.run.spectrumList.spectrum[i - 1].binaryDataArrayList.binaryDataArray[4].userParam[0] = new Generated.UserParamType()
                    {
                        name = "kelleherCustomType",
                        value = "noise intensity",
                    };
                }
            }

            if (!writeIndexed)
            {
                using (TextWriter writer = new StreamWriter(outputFile))
                {

                    mzmlSerializer.Serialize(writer, mzML);
                }
            }

            else if (writeIndexed)
            {

                var inMemoryTextWriter = new System.IO.MemoryStream();


                //compute total offset
                indexedMzml.mzML = mzML;


                indexedSerializer.Serialize(inMemoryTextWriter, indexedMzml);
                string allmzMLData = Encoding.UTF8.GetString(inMemoryTextWriter.ToArray());
                int p = allmzMLData.Length;
                string removedBreaks = allmzMLData.Replace("\r\n", "\n");
                long? indexListOffset = removedBreaks.Length;


                //new stream with correct formatting
                var memStreamWithLineEndings = new MemoryStream(Encoding.UTF8.GetBytes(removedBreaks));


                indexedMzml.indexList = new Generated.IndexListType()
                {
                    count = "2",
                    index = new Generated.IndexType[2]

                };
                var indexname = new Generated.IndexTypeName();

                //spectra naming
                indexedMzml.indexList.index[0] = new Generated.IndexType()
                {
                    name = indexname,


                };

                indexname = Generated.IndexTypeName.chromatogram;
                indexedMzml.indexList.index[0].name = new Generated.IndexTypeName()
                {

                };


                //chroma naming
                indexedMzml.indexList.index[1] = new Generated.IndexType()
                {
                    name = indexname,

                };


                indexedMzml.indexList.index[1].name = indexname;
                {

                };

                int numScans = myMsDataFile.NumSpectra;
                int numChromas = Int32.Parse(mzML.run.chromatogramList.count);

                //now calculate offsets of each scan and chroma

                //add spectra offsets
                indexedMzml.indexList.index[0].offset = new Generated.OffsetType[numScans];
                //add chroma offsets
                indexedMzml.indexList.index[1].offset = new Generated.OffsetType[numChromas];


                var reader = new StreamReader(memStreamWithLineEndings, Encoding.UTF8);
                memStreamWithLineEndings.Position = 0;

                int i = 0;
                int a = 1;
                int index;
                //spectra offset loop
                while (i < numScans)
                {
                    index = removedBreaks.IndexOf(mzML.run.spectrumList.spectrum[i].id, (a - 1));
                    if (index != -1)
                    {
                        a = index;
                        indexedMzml.indexList.index[0].offset[i] = new Generated.OffsetType();
                        indexedMzml.indexList.index[0].offset[i].idRef = mzML.run.spectrumList.spectrum[i].id;
                        indexedMzml.indexList.index[0].offset[i].Value = a + 3;
                        i++;
                    }

                }

                index = removedBreaks.IndexOf("id=\"" + mzML.run.chromatogramList.chromatogram[0].id + "\"", (a - 1));
                if (index != -1)
                {
                    a = index;
                    indexedMzml.indexList.index[1].offset[0] = new Generated.OffsetType();
                    indexedMzml.indexList.index[1].offset[0].idRef = mzML.run.chromatogramList.chromatogram[0].id;
                    indexedMzml.indexList.index[1].offset[0].Value = a + 3;
                }

                indexedMzml.indexListOffset = indexListOffset - 32;
                // 4. Write indexed mzML object to string in memory


                string chksum = "";

                //Write to file

                TextWriter writer = new StreamWriter(outputFile);

                writer.NewLine = "\n";
                writer.Close();
                var stream = File.OpenRead(outputFile);
                //compute checksum
                chksum = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(stream));
                chksum = chksum.Replace("-", String.Empty);
                chksum = chksum.ToLower();
                indexedMzml.fileChecksum = chksum;
                stream.Close();
                writer = new StreamWriter(outputFile);
                writer.NewLine = "\n";
                indexedSerializer.Serialize(writer, indexedMzml);
                writer.Close();




            }

            #endregion Public Methods

        }
    }

    public class TrackTextPosition : TextReader
    {
        private TextReader _baseReader;
        private int _position;

        public TrackTextPosition(TextReader baseReader)
        {
            _baseReader = baseReader;
        }

        public override int Read()
        {
            _position++;
            return _baseReader.Read();
        }

        public override int Peek()
        {
            return _baseReader.Peek();
        }

        public int Position
        {
            get { return _position; }
        }
    }
}
