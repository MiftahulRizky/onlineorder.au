<%@ Page Title="Roman Blinds Order" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="RomanBlinds.aspx.vb" Inherits="Order_RomanBlinds" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                        <span id="pageAction">. . . . .</span>
                    </div>
                    <h2 class="page-title" id="pageTitle">. . . . .</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
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
                                        <div class="col-lg-8 " id="divOrderNo">. . . . .</div>
                                    </div>
                                    <div class=" row">
                                        <label class="col-lg-3">REFERENCE </label>
                                        <div class="col-lg-1 ">:</div>
                                        <div class="col-lg-8 " id="divOrderCust">. . . . .</div>
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
                                <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">roman type</label>
                                <div class="col-lg-5">
                                    <select type="text" name="blindtype" id="blindtype" class="form-control"></select>
                                </div>  
                            </div>

                            <div class="mb-3 row" id="divControlType">
                                <label for="controltype" class="col-lg-3 text-uppercase fw-bold">control type</label>
                                <div class="col-lg-5">
                                    <select type="text" name="controltype" id="controltype" class="form-control "></select>
                                </div>  
                            </div>

                            <div id="divFormDetail">
                                <hr>
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

                                <div class="mb-3 row">
                                    <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold">fabric type x colour</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select name="fabrictype" id="fabrictype" class="form-control "></select>
                                        <small class="form-hint">* Type</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select name="fabriccolour" id="fabriccolour" class="form-control "></select>
                                        <small class="form-hint">* Colour</small>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="width" class="col-lg-3 text-uppercase fw-bold">width x drop</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                    <div class="input-group">
                                        <input type="number" min="1" name="width" id="width" class="form-control " autocomplete="off" placeholder="Width ...." />
                                        <span class="input-group-text ">mm</span>
                                    </div>
                                        <small class="form-hint">* Width</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Drop ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Drop</small>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="controlposition" class="col-lg-3 text-uppercase fw-bold">control position</label>
                                    <div class="col-lg-3">
                                        <select name="controlposition" id="controlposition" class="form-control "></select>
                                    </div>
                                </div>

                                <div id="divChained">
                                    <div class="mb-3 row">
                                        <label for="materialchain" class="col-lg-3 text-uppercase fw-bold">material chain</label>
                                        <div class="col-lg-3">
                                            <select name="materialchain" id="materialchain" class="form-control "></select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="chaincolour" class="col-lg-3 text-uppercase fw-bold">chain colour x length</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="chaincolour" id="chaincolour" class="form-control "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="chainlength" id="chainlength" class="form-control  " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                    </div>

                                </div><!-- /divChained -->

                                <div id="divCordlock">

                                    <div class="mb-3 row">
                                        <label for="cordcolour" class="col-lg-3 text-uppercase fw-bold">cord colour x length</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="cordcolour" id="cordcolour" class="form-control "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="cordlength" id="cordlength" class="form-control  " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                    </div>

                                </div><!-- /divCordlock -->

                                <div class="mb-3 row" id="divBattenColour">
                                    <label for="battencolour" class="col-lg-3 text-uppercase fw-bold">batten colour</label>
                                    <div class="col-lg-4">
                                        <select name="battencolour" id="battencolour" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divPlasticColour">
                                    <label for="plasticcolour" class="col-lg-3 text-uppercase fw-bold">acorn plastic colour</label>
                                    <div class="col-lg-4">
                                        <select name="plasticcolour" id="plasticcolour" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divCleat">
                                    <label for="cleat" class="col-lg-3 text-uppercase fw-bold">cleat</label>
                                    <div class="col-lg-4">
                                        <select name="cleat" id="cleat" class="form-control "></select>
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
                                
                            </div><!-- /divFormDetail -->

                        </div>
                        <div class="card-footer text-end">
                           <button type="button" id="btnSubmit" class="btn btn-primary ">plain text</button>
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




    <script type="text/javascript">
        let designIdOri = '7120F117-F60A-4EC8-B092-4E44C8386B37';
        let headerId = '<%=Session("HeaderId")%>'
        let itemAction = '<%= Session("itemAction") %>';
        let designId = '<%= Session("designId") %>';
        let itemId = '<%= Session("itemId") %>';
        let userId = '<%= Session("userId") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';
        let markupAccess = '<%= Session("MarkUpAccess") %>';
        let uriMethod = "/Methods/Order/RomanBlindMethod.aspx"
        
    </script>
    <script src="/Scripts/Order/RomanBlinds.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

