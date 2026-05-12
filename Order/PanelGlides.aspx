<%@ Page Title="Panel Glides Order" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PanelGlides.aspx.vb" Inherits="Order_PanelGlides" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                <div class="col-lg-7 col-md-7 col-sm-8">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-4 col-lg-4 col-md-4">
                                    <label class="form-label text-secondary text-uppercase">Order #</label>
                                    <label class="form-label" id="lblOrder">-</label>
                                    <label class="form-label" id="lblItemId">-</label>
                                </div>
                                <div class="col-4 col-lg-4 col-md-4">
                                    <label class="form-label text-secondary text-uppercase">Order Number</label>
                                    <label class="form-label" id="lblOrderNumber">-</label>
                                </div>
                                <div class="col-4 col-lg-4 col-md-4">
                                    <label class="form-label text-secondary text-uppercase">Order Name</label>
                                    <label class="form-label" id="lblOrderName">-</label>
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
                                    <select type="text" name="blindtype" id="blindtype" class="form-control"></select>
                                </div>  
                            </div>

                            <div class="mb-3 row" id="divColourType" hidden>
                                <label for="colourtype" class="col-lg-3 text-uppercase fw-bold">colour</label>
                                <div class="col-lg-5">
                                    <select type="text" name="colourtype" id="colourtype" class="form-control "></select>
                                </div>  
                            </div>

                            <div id="divFormDetail">
                                <hr/>
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
                                    <label for="width" class="col-lg-3 text-uppercase fw-bold" id="lblWidthDrop">width x drop</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12" id="divWidth">
                                        <div class="input-group">
                                            <input type="number" min="1" name="width" id="width" class="form-control " autocomplete="off" placeholder="Width ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Width</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12" id="divDrop">
                                        <div class="input-group">
                                            <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Drop ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Drop</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divFabric">
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

                                

                                <div class="mb-3 row" id="divLayoutCode">
                                    <label for="layoutcode" class="col-lg-3 text-uppercase fw-bold">layout code</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select name="layoutcode" id="layoutcode" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divNoPanel">
                                    <label for="nopanel" class="col-lg-3 text-uppercase fw-bold">no of panel</label>
                                    <div class="col-lg-2">
                                        <select name="nopanel" id="nopanel" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divTrack">
                                    <label for="tracktype" class="col-lg-3 text-uppercase fw-bold">track type x colour</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select name="tracktype" id="tracktype" class="form-control "></select>
                                        <small class="form-hint">* Type</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select name="trackcolour" id="trackcolour" class="form-control "></select>
                                        <small class="form-hint">* Colour</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divWandPosition">
                                    <label for="wandposition" class="col-lg-3 text-uppercase fw-bold">wand position</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select name="wandposition" id="wandposition" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divWand">
                                    <label for="wandlength" class="col-lg-3 text-uppercase fw-bold">wand length x colour</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="0" name="wandlength" id="wandlength" class="form-control " autocomplete="off">
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        <small class="form-hint">* Length</small>
                                    </div>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select name="wandcolour" id="wandcolour" class="form-control "></select>
                                        <small class="form-hint">* Colour</small>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divBottomRail">
                                    <label for="bottomrail" class="col-lg-3 text-uppercase fw-bold">bottom rail</label>
                                    <div class="col-lg-4">
                                        <select name="bottomrail" id="bottomrail" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divBatten" hidden>
                                    <label for="batten" class="col-lg-3 text-uppercase fw-bold">batten</label>
                                    <div class="col-lg-2">
                                        <select name="batten" id="batten" class="form-control ">
                                            <option value=""></option>
                                            <option value="No">NO</option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divBattenColour" hidden>
                                    <label for="battencolour" class="col-lg-3 text-uppercase fw-bold">batten colour</label>
                                    <div class="col-lg-4">
                                        <select name="battencolour" id="battencolour" class="form-control "></select>
                                    </div>
                                </div>

                                <div class="mb-3 row" id="divFitting" hidden>
                                    <label for="fitting" class="col-lg-3 text-uppercase fw-bold">fitting</label>
                                    <div class="col-lg-2">
                                        <select name="fitting" id="fitting" class="form-control ">
                                            <option value=""></option>
                                            <option value="Reveal">REVEAL</option>
                                            <option value="Face">FACE</option>
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
                            <button type="button" id="btnSubmit" class="btn btn-primary ">Submit</button>
                            <a href="javascript:void(0);" id="btnCancel"  class="btn btn-danger " >Cancel
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
        // Sunlight
        let DESIGNIDORI = 'AC917815-434A-4FD2-BE06-C39319863D5B';
        let HEADERID = '<%=Session("headerId")%>'
        let ORDERTYPE = '<%= Session("orderType") %>';
        let ITEMACTION = '<%= Session("itemAction") %>';
        let DESIGNID = '<%= Session("designId") %>';
        let ITEMID = '<%= Session("itemId") %>';
        let LOGINID = '<%= Session("LoginId") %>';
        let ROLENAME = '<%= Session("RoleName") %>';
        let LEVELNAME = '<%= Session("LevelName") %>';
        let MARKUPACCESS = '<%= Session("MarkUpAccess") %>';
        let URIMETHOD = "/Methods/Order/PanelGlideMethod.aspx"
        
    </script>
    <script src="/Scripts/Order/PanelGlides.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

