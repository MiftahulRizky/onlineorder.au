<%@ Page Language="VB" AutoEventWireup="true" CodeFile="PanoramaShutters.aspx.vb" Inherits="Order_PanoramaShutters" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Panorama Shutters Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <style>

        #loading-overlay {
            transition: opacity 0.5s ease;
            opacity: 1;
        }
        #loading-overlay.fade-out {
            opacity: 0;
        }
      

    </style>

    <!-- Loading Overlay -->
    <div id="loading-overlay"
        class="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center bg-body bg-opacity-85"
        style="z-index: 1050;">
        <div class="page page-center w-100 h-100">
            <div class="container container-slim py-4 h-100 d-flex flex-column justify-content-center">
                <div class="text-center">
                    <div class="mb-3">
                        <a href="javascript:void(0)" class="navbar-brand navbar-brand-autodark">
                            <img src="http://10.0.209.168:8888/Content/static/new-icon.png" height="36" alt="">
                        </a>
                    </div>
                    <div class="text-secondary mb-3">Preparing application</div>
                    <div class="progress progress-sm">
                        <div class="progress-bar progress-bar-indeterminate"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    

    <div class="page-header mb-4 ">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                        <span  id="pageAction">.............</span>
                    </div>
                    <h2 class="page-title"  id="pageTitle">.............</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="body-page">
        <div class="container-xl" id="pageContent">

            <div class="row mb-3">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-6">
                                    <div class=" row mb-2">
                                        <label class="col-lg-3">ORDER NUMBER</label>
                                        <div class="col-lg-1 ">:</div>
                                        <div class="col-lg-8 " id="divOrderNo">.............</div>
                                    </div>
                                    <div class=" row">
                                        <label class="col-lg-3">REFERENCE </label>
                                        <div class="col-lg-1 ">:</div>
                                        <div class="col-lg-8 " id="divOrderCust">.............</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            
            <!-- element input -->
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="cardTitle"></h3>
                        </div>
                        <div class="card-body">

                            <div class="mb-3 row">
                                <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">type</label>
                                <div class="col-lg-5">
                                    <select type="text" name="blindtype" id="blindtype" class="form-control "></select>
                                </div>  
                            </div>

                            <div class="mb-3 row" id="divColourType">
                                <label for="colourtype" class="col-lg-3 text-uppercase fw-bold">colour</label>
                                <div class="col-lg-5">
                                    <select type="text" name="colourtype" id="colourtype" class="form-control "></select>
                                </div>  
                            </div>
                            
                            <div id="divFormDetail">
                                <hr />
                                <div class="mb-3 row">
                                    <label for="qty" class="col-lg-3 text-uppercase fw-bold">quantity</label>
                                    <div class="col-lg-2">
                                        <input type="number" min="1" name="qty" id="qty" class="form-control " value="1" autocomplete="off">
                                    </div>
                                </div>
                                
                                <div class="mb-3 row">
                                    <label for="room" class="col-lg-3 text-uppercase fw-bold">room to install</label>
                                    <div class="col-lg-5">
                                        <input " name="room" id="room" class="form-control "autocomplete="off">
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="mounting" class="col-lg-3 text-uppercase fw-bold">Mounting</label>
                                    <div class="col-lg-3">
                                        <select name="mounting" id="mounting" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divSemiInsideMount">
                                    <label for="semiinsidemount" class="col-lg-3 text-uppercase fw-bold">semi inside mount</label>
                                    <div class="col-lg-3">
                                        <select name="semiinsidemount" id="semiinsidemount" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row">
                                    <label for="width" class="col-lg-3 text-uppercase fw-bold">width x height</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                    <div class="input-group">
                                        <input type="number" min="1" name="width" id="width" class="form-control " autocomplete="off" placeholder="Width ...." />
                                        <span class="input-group-text ">mm</span>
                                    </div>
                                        <small class="form-hint">* Width</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Height ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="louvresize" class="col-lg-3 text-uppercase fw-bold">louvre size</label>
                                    <div class="col-lg-3">
                                        <select name="louvresize" id="louvresize" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divLouvrePosition">
                                    <label for="louvreposition" class="col-lg-3 text-uppercase fw-bold">louvre position</label>
                                    <div class="col-lg-3">
                                        <select name="louvreposition" id="louvreposition" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="midrailheight1" class="col-lg-3 text-uppercase fw-bold">midrail height</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="1" id="midrailheight1" class="form-control " autocomplete="off" placeholder="Height 1" />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 1</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12" id="divMidrailHeight2">
                                        <div class="input-group">
                                            <input type="number" min="1" id="midrailheight2" class="form-control  " autocomplete="off" placeholder="Height 2" />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 2</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divMidrailCritical">
                                    <label for="midrailcritical" class="col-lg-3 text-uppercase fw-bold">critical midrail</label>
                                    <div class="col-lg-3">
                                        <select name="midrailcritical" id="midrailcritical" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divPanelQty">
                                    <label for="panelqty" class="col-lg-3 text-uppercase fw-bold">panel qty</label>
                                    <div class="col-lg-2">
                                        <select name="panelqty" id="panelqty" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divJoinedPanels">
                                    <label for="joinedpanels" class="col-lg-3 text-uppercase fw-bold">co-joined panels</label>
                                    <div class="col-lg-2">
                                        <select name="joinedpanels" id="joinedpanels" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divHingeColour">
                                    <label for="hingecolour" class="col-lg-3 text-uppercase fw-bold">hinge colour</label>
                                    <div class="col-lg-3">
                                        <select name="hingecolour" id="hingecolour" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divCustomHeaderLength">
                                    <label for="customheaderlength" class="col-lg-3 text-uppercase fw-bold">custom header</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="1" id="customheaderlength" class="form-control  " autocomplete="off" placeholder="Length ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Length</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divLayoutCode">
                                    <label for="layoutcode" class="col-lg-3 text-uppercase fw-bold">layout code</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select name="layoutcode" id="layoutcode" class="form-control "></select>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12" id="divLayoutCodeCustom">
                                        <input type="text" id="layoutcodecustom" class="form-control  " autocomplete="off" placeholder="Custom ...." />
                                        <small class="form-hint">* Custom Layout Code (Layout Other)</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divFrameType">
                                    <label for="frametype" class="col-lg-3 text-uppercase fw-bold">frame type</label>
                                    <div class="col-lg-6">
                                        <div class="input-group">
                                            <select name="frametype" id="frametype" class="form-control "></select>
                                            <button class="btn  btn-show-info" type="button" data-params="frametype">?</button>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divFrameLeft">
                                    <label for="frameleft" class="col-lg-3 text-uppercase fw-bold">frame left</label>
                                    <div class="col-lg-3">
                                        <select name="frameleft" id="frameleft" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divFrameRight">
                                    <label for="frameright" class="col-lg-3 text-uppercase fw-bold">frame right</label>
                                    <div class="col-lg-3">
                                        <select name="frameright" id="frameright" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divFrameTop">
                                    <label for="frametop" class="col-lg-3 text-uppercase fw-bold">top frame</label>
                                    <div class="col-lg-3">
                                        <select name="frametop" id="frametop" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divFrameBottom">
                                    <label for="framebottom" class="col-lg-3 text-uppercase fw-bold">bottom frame</label>
                                    <div class="col-lg-3">
                                        <select name="framebottom" id="framebottom" class="form-control "></select>
                                    </div>
                                </div>
                                
                                
                                <div class="mb-3 row" id="divBottomTrackType">
                                    <label for="bottomtracktype" class="col-lg-3 text-uppercase fw-bold">bottom track type</label>
                                    <div class="col-lg-3">
                                        <select name="bottomtracktype" id="bottomtracktype" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divBottomTrackRecess">
                                    <label for="bottomtrackrecess" class="col-lg-3 text-uppercase fw-bold">bottom track recess</label>
                                    <div class="col-lg-2">
                                        <select name="bottomtrackrecess" id="bottomtrackrecess" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divBuildout">
                                    <label for="buildout" class="col-lg-3 text-uppercase fw-bold">buildout</label>
                                    <div class="col-lg-3">
                                        <select name="buildout" id="buildout" class="form-control "></select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divBuildoutPosition">
                                    <label for="buildoutposition" class="col-lg-3 text-uppercase fw-bold">buildout position</label>
                                    <div class="col-lg-3">
                                        <select name="buildoutposition" id="buildoutposition" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divSameSize">
                                    <label for="samesizepanel" class="col-lg-3 text-uppercase fw-bold">same size panel</label>
                                    <div class="col-lg-3">
                                        <select name="samesizepanel" id="samesizepanel" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divGapPost">
                                    <label for="" class="col-lg-3 text-uppercase fw-bold">gap / t-post</label>
                                    <div class="col-lg-9 row">
                                        <div class="col-lg-4" id="divGap1">
                                            <div class="input-group ">
                                                <input type="number" min="1" name="gap1" id="gap1" class="form-control  gaps" placeholder="Gap 1">
                                                <button class="btn  btn-show-info" type="button" data-params="gap">?</button>
                                            </div>
                                            <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 1</small>
                                        </div>
                                        <div class="col-lg-4" id="divGap2">
                                            <div class="input-group ">
                                                <input type="number" min="1" name="gap2" id="gap2" class="form-control  gaps" placeholder="Gap 2">
                                                <button class="btn  btn-show-info" type="button" data-params="gap">?</button>
                                            </div>
                                            <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 2</small>
                                        </div>
                                        <div class="col-lg-4" id="divGap3">
                                            <div class="input-group ">
                                                <input type="number" min="1" name="gap3" id="gap3" class="form-control  gaps" placeholder="Gap 3">
                                                <button class="btn  btn-show-info" type="button" data-params="gap">?</button>
                                            </div>
                                            <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 3</small>
                                        </div>
                                        <div class="col-lg-4 mt-3" id="divGap4">
                                            <div class="input-group " >
                                                <input type="number" min="1" name="gap4" id="gap4" class="form-control  gaps" placeholder="Gap 4">
                                                <button class="btn  btn-show-info" type="button" data-params="gap">?</button>
                                            </div>
                                            <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 4</small>
                                        </div>
                                        <div class="col-lg-4 mt-3" id="divGap5">
                                            <div class="input-group ">
                                                <input type="number" min="1" name="gap5" id="gap5" class="form-control  gaps" placeholder="Gap 5">
                                                <button class="btn  btn-show-info" type="button" data-params="gap">?</button>
                                            </div>
                                            <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 5</small>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divHorizontalTPost">
                                    <label for="horizontaltpostheight" class="col-lg-3 text-uppercase fw-bold">horizontal t-post</label>
                                    <div class="col-lg-3">
                                        <div class="input-group">
                                            <input type="number" min="1" name="horizontaltpostheight" id="horizontaltpostheight" class="form-control " placeholder=".......">
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height</small>
                                    </div>

                                    <div class="col-lg-3" id="divHorizontalTPostRequired">
                                        <select name="horizontaltpost" id="horizontaltpost" class="form-control "></select>
                                        <small class="form-hint">* Required</small>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divTiltrodType">
                                    <label for="tiltrodtype" class="col-lg-3 text-uppercase fw-bold">tiltrod type</label>
                                    <div class="col-lg-3">
                                        <div class="input-group">
                                            <select name="tiltrodtype" id="tiltrodtype" class="form-control "></select>
                                            <button class="btn  btn-show-info" type="button" data-params="tiltrodtype">?</button>
                                        </div>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divTiltrodSplit">
                                    <label for="tiltrodsplit" class="col-lg-3 text-uppercase fw-bold">tiltrod rotation</label>
                                    <div class="col-lg-7">
                                        <select name="tiltrodsplit" id="tiltrodsplit" class="form-control "></select>
                                        <small class="form-hint">* Split Tiltrod Rotation</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divTiltrodHeight">
                                    <label for="splitheight1" class="col-lg-3 text-uppercase fw-bold">split height</label>
                                    <div class="col-lg-3">
                                        <div class="input-group">
                                            <input type="number" min="1" name="splitheight1" id="splitheight1" class="form-control " placeholder=".......">
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 1 </small>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="input-group">
                                            <input type="number" min="1" name="splitheight2" id="splitheight2" class="form-control " placeholder=".......">
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 2</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divReverseHinged">
                                    <label for="reversehinged" class="col-lg-3 text-uppercase fw-bold">reverse hinged</label>
                                    <div class="col-lg-2">
                                        <select name="reversehinged" id="reversehinged" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divPelmetFlat">
                                    <label for="pelmetflat" class="col-lg-3 text-uppercase fw-bold">pelmet flat packed</label>
                                    <div class="col-lg-2">
                                        <select name="pelmetflat" id="pelmetflat" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divExtraFascia">
                                    <label for="extrafascia" class="col-lg-3 text-uppercase fw-bold">extra fascia</label>
                                    <div class="col-lg-2">
                                        <select name="extrafascia" id="extrafascia" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divHingesLoose">
                                    <label for="hingesloose" class="col-lg-3 text-uppercase fw-bold">hinges loose</label>
                                    <div class="col-lg-2">
                                        <select name="hingesloose" id="hingesloose" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divCutOut">
                                    <label for="cutout" class="col-lg-3 text-uppercase fw-bold">french door cut-out</label>
                                    <div class="col-lg-2">
                                        <select name="cutout" id="cutout" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divSpecialShape">
                                    <label for="specialshape" class="col-lg-3 text-uppercase fw-bold">special shape</label>
                                    <div class="col-lg-2">
                                        <select name="specialshape" id="specialshape" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divTemplateProvided">
                                    <label for="templateprovided" class="col-lg-3 text-uppercase fw-bold">template provided</label>
                                    <div class="col-lg-2">
                                        <select name="templateprovided" id="templateprovided" class="form-control ">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" >
                                    <label for="notes" class="col-lg-3 text-uppercase fw-bold">special information</label>
                                    <div class="col-lg-8">
                                        <textarea name="notes" id="notes" class="form-control" placeholder="Your notes ..." rows="6" style="resize: none;"></textarea>
                                        <span class="form-label-description" id="notescount">0/1000</span>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row" id="divMarkUp">
                                    <label for="notes" class="col-lg-3 text-uppercase fw-bold">mark up</label>
                                    <div class="col-lg-3">
                                        <div class="input-group">
                                            <input type="number" min="0" name="markup" id="markup" class="form-control "  autocomplete="off">
                                            <span class="input-group-text ">%</span>
                                        </div>
                                    </div>
                                </div>



                            </div>
                        </div>

                        <div class="card-footer text-end">
                            <button type="button" id="btnSubmit" class="btn btn-primary "></button>
                            <a href="javascript:void(0);" id="btnCancel"  class="btn btn-danger " >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </a>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <p runat="server" id="pNotes"></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


        </div>
    </div>

    <div class="modal modal-blur fade" id="modalInfo" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalTitle">Modal Title</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="text-secondary">
                        <span id="spanInfo"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn btn-secondary w-100" data-bs-dismiss="modal">OK</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <script type="text/javascript">
        let designIdShutters = '7739272E-1DAC-457E-B05C-81A60E711974';
        let headerId = '<%=Session("HeaderId")%>'
        let itemAction = '<%= Session("itemAction") %>';
        let designId = '<%= Session("designId") %>';
        let itemId = '<%= Session("itemId") %>';
        let userId = '<%= Session("userId") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';
        let markupAccess = '<%= Session("MarkUpAccess") %>';
        let uriMethod = "/Methods/Order/PanoramaShutterMethod.aspx";
        
    </script>
    <script src="/Scripts/Order/PanoramaShutters.js?<%= DateTime.Now.Ticks %>"></script>

</asp:Content>

