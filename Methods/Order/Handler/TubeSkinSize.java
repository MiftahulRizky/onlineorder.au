TubeWidthRoller     Routine

!Validate Tube Width 
!Roller Standard
    CASE ORDD:DesignType
    OF 'Roller' OROF 'Holland'
        CASE STO:CompanyGroup
        OF 'SP' !(Selain SBS, Accent, JPMD)
            !JAI SYSTEM
            IF INSTRING('JAI System', HK:KitName, 1, 1)
                !JAI (D.L.S) DEP
                IF INSTRING('(D.L.S) Dep', HK:KitName, 1, 1) !D.L.S
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                        END
                    END
                END !(D.L.S) DEP

                !JAI (D.L.S) IND
                IF INSTRING('(D.L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END 
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                        END
                    END
                END !(D.L.S) IND

                !JAI (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40' 
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                        END
                    END
                END !(DR)

                !JAI (L.S) DEP
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1) !L.S
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                        END
                    END
                END !(L.S) DEP

                !JAI (L.S) IND
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END 
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END                    
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40' 
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                        END
                    END
                END !(L.S) IND
                !JAI (S)

                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 28
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 32
                                END
                            END
                        END
                    END

                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize

                            IF ORDD:TubeType = '40' 
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END

                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 26
                            END

                            IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END

                            IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                                IF ORDD:TubeType = '40'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                                IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                    ORDD:TubeSkinSize = ORDD:Width - 26
                                END
                            END
                            
                        END
                    END
                END !(S)

            END !JAI

            
            !Sunboss OR Skyline
            IF INSTRING('Skyline', HK:KitName, 1, 1) OR INSTRING('Sunboss', HK:KitName, 1, 1)
                !Sunbos (D.L.S) Dep
                IF INSTRING('(D.L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                        END
                    END
                END !(D.L.S) DEP     

                !Sunboss (D.L.S) Ind
                IF INSTRING('(D.L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                        END
                    END
                END !(D.L.S) IND

                !Sunboss (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                        END
                    END
                END !(DR)

                !Sunboss (L.S) Dep
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                        END
                    END
                END !(L.S) DEP

                !Sunboss (L.S) Ind
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 22
                            END
                        END
                    END
                END !(L.S) IND

                !Sunboss (S)
                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ~ORDD:CustomSize
                            IF ORDD:TubeType = '50'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                        END
                        IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                            IF ~ORDD:CustomSize
                                IF ORDD:TubeType = '50'
                                    ORDD:TubeSkinSize = ORDD:Width - 22
                                END
                            END
                        END
                    END
                END !(S)

            END !SUNBOSS

            !SPRING SYSTEM CONDITION
            IF INSTRING('Spring System', HK:KitName, 1, 1)
                IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                    IF ~ORDD:CustomSize
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END                   
                    END
                END
            END !SPRING SYSTEM

            
        OF 'INT' OROF '' !(SBS, Accent, JPMD)
            IF INSTRING('Ventura', HK:KitName, 1, 1)

                !Ventura (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END    
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END    
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END  
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END    
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                        END
                    END

                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END         
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                END

                !Ventura (L.S) Dep
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1)

                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END       
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END                           
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END                            
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END

                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                END

                !Ventura (L.S) Ind
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END      
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END     
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                END

                !Ventura (S)
                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END      
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                END
            END
            !JAI SYSTEM
            IF INSTRING('JAI System', HK:KitName, 1, 1)
                !JAI (D.L.S) Dep
                IF INSTRING('(D.L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END       
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                         IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
                !JAI (D.L.S) Ind
                IF INSTRING('(D.L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END        
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
                !JAI (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END    
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
                !JAI (L.S) DEP
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                            IF ORDD:TubeType = '40'
                                ORDD:TubeSkinSize = ORDD:Width - 28
                            END
                            IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                ORDD:TubeSkinSize = ORDD:Width - 32
                            END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 22 !-20
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
                !JAI (L.S) Ind
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40'
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
                !JAI (S)
                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 28
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 32
                             END
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 26
                        END
                        IF ORDD:BracketType = 'JAI Com' OR ORDD:BracketType = 'JAI D Cover' OR ORDD:BracketType = 'JAI DL 4B2C' OR ORDD:BracketType = 'JAI DL 4B4C' OR ORDD:BracketType = 'JAI Double' OR ORDD:BracketType = 'JAI Ex Cover' OR ORDD:BracketType = 'JAI Ex SP Cover' OR ORDD:BracketType = 'JAI EXL 2B1C' OR ORDD:BracketType = 'JAI EXL 2B2C' OR ORDD:BracketType = 'JAI Extension' OR ORDD:BracketType = 'JAI L 2B1C' OR ORDD:BracketType = 'JAI L 2B2C' OR ORDD:BracketType = 'JAI N/A' OR ORDD:BracketType = 'JAI S Cover' OR ORDD:BracketType = 'JAI Single' OR ORDD:BracketType = 'JAI Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'JAI G Com' OR ORDD:BracketType = 'JAI G D Cover' OR ORDD:BracketType = 'JAI G DL 4B2C' OR ORDD:BracketType = 'JAI G DL 4B4C' OR ORDD:BracketType = 'JAI G Double' OR ORDD:BracketType = 'JAI G Ex Cover' OR ORDD:BracketType = 'JAI G Ex SP Cover' OR ORDD:BracketType = 'JAI G EXL 2B1C' OR ORDD:BracketType = 'JAI G EXL 2B2C' OR ORDD:BracketType = 'JAI G Extension' OR ORDD:BracketType = 'JAI G L 2B1C' OR ORDD:BracketType = 'JAI G L 2B2C' OR ORDD:BracketType = 'JAI G N/A' OR ORDD:BracketType = 'JAI G S Cover' OR ORDD:BracketType = 'JAI G Single' OR ORDD:BracketType = 'JAI G Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                        IF ORDD:BracketType = 'A JAI G Com' OR ORDD:BracketType = 'A JAI G D Cover' OR ORDD:BracketType = 'A JAI G DL 4B2C' OR ORDD:BracketType = 'A JAI G DL 4B4C' OR ORDD:BracketType = 'A JAI G Double' OR ORDD:BracketType = 'A JAI G Ex Cover' OR ORDD:BracketType = 'A JAI G Ex SP Cover' OR ORDD:BracketType = 'A JAI G EXL 2B1C' OR ORDD:BracketType = 'A JAI G EXL 2B2C' OR ORDD:BracketType = 'A JAI G Extension' OR ORDD:BracketType = 'A JAI G L 2B1C' OR ORDD:BracketType = 'A JAI G L 2B2C' OR ORDD:BracketType = 'A JAI G N/A' OR ORDD:BracketType = 'A JAI G S Cover' OR ORDD:BracketType = 'A JAI G Single' OR ORDD:BracketType = 'A JAI G Split Cover'
                             IF ORDD:TubeType = '40' 
                                 ORDD:TubeSkinSize = ORDD:Width - 22
                             END
                             IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                                 ORDD:TubeSkinSize = ORDD:Width - 26
                             END
                        END
                    END
                END
            END
            !Monarch
            IF INSTRING('Monarch', HK:KitName, 1, 1)
                !Monarch (D.L.S) Dep
                IF INSTRING('(D.L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END  
                    END
                END
                !Monarch (D.L.S) Ind
                IF INSTRING('(D.L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END 
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                END
                !Monarch (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                END
                !Monarch (L.S) Dep
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 20
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                END
                !Monarch (L.S) Ind
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END  
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                END
                !Monarch (S)
                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '40' 
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '40'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END    
                        IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                            ORDD:TubeSkinSize = ORDD:Width - 32
                        END
                    END
                END
            END
            !Sunboss
            IF INSTRING('Skyline', HK:KitName, 1, 1) OR INSTRING('Sunboss', HK:KitName, 1, 1) 
                !Sunboss (D.L.S) Dep
                IF INSTRING('(D.L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
                !Sunboss (D.L.S) Ind
                IF INSTRING('(D.L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
                !Sunboss (DR)
                IF INSTRING('(DR)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
                !Sunboss (L.S) Dep
                IF INSTRING('(L.S) Dep', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
                !Sunboss (L.S) Ind
                IF INSTRING('(L.S) Ind', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
                !Sunboss (S)
                IF INSTRING('(S)', HK:KitName, 1, 1)
                    IF ORDD:ControlPosition = 'RHC' OR ORDD:ControlPosition = 'LHC'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 28
                        END
                    END
                    IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                        IF ORDD:TubeType = '50'
                            ORDD:TubeSkinSize = ORDD:Width - 22
                        END
                    END
                END
            END
            !SPRING SYSTEM CONDITION
            IF INSTRING('Spring System', HK:KitName, 1, 1)
                IF ORDD:ControlPosition = '' OR ORDD:ControlPosition = 'N/A'
                    IF ORDD:TubeType = '40'
                        ORDD:TubeSkinSize = ORDD:Width - 28
                    END  
                    IF ORDD:TubeType = '45' OR ORDD:TubeType = '45H'
                        ORDD:TubeSkinSize = ORDD:Width - 32
                    END
                END
            END
        END

    END
    Do TubeWidthOverride 