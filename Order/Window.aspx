<%@ Page Title="Window" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Window.aspx.vb" Inherits="Order_Window" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                <div class="col-lg-7 col-md-7 col-sm-7">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-4 col-lg-4 col-md-4">
                                    <label class="form-label text-secondary text-uppercase">Order #</label>
                                    <label class="form-label" id="lblOrder">-</label>
                                    <label class="form-label" id="lblItemId" hidden>-</label>
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

            <div class="row mb-3">

                <div class="col-7 col-lg-7 col-md-12">
                    <div class="card">
                        <form action="javascript:void(0)" method="post" id="formSubmit">
                            <div class="card-header">
                                <h3 class="card-title" id="cardTitle"></h3>
                            </div>
                            <div class="card-body">
                                
                                <div class="mb-3 row">
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">window type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblTubeType">product type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="tubetype" id="tubetype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div  id="divFormDetail">
                                    <hr/>

                                    <div class="mb-3 row" >
                                        <label for="qty" class="col-lg-3 text-uppercase fw-bold" >quantity</label>
                                        <div class="col-lg-4">
                                            <input type="number" min="1" value="1" name="qty" id="qty" class="form-control">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" >
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room / location</label>
                                        <div class="col-lg-4">
                                            <input type="text" name="room" id="room" class="form-control" autocomplete="off">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMounting">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >mounting</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-select"></select>
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

                                    <div class="mb-3 row" id="divMesh">
                                        <label for="meshtype" class="col-lg-3 text-uppercase fw-bold" >mesh type</label>
                                        <div class="col-lg-4">
                                            <select name="meshtype" id="meshtype" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSlidingType">
                                        <label for="slidingtype" class="col-lg-3 text-uppercase fw-bold" >sliding type</label>
                                        <div class="col-lg-4">
                                            <select name="slidingtype" id="slidingtype" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divStacking">
                                        <label for="stacking" class="col-lg-3 text-uppercase fw-bold" >stacking</label>
                                        <div class="col-lg-4">
                                            <select name="stacking" id="stacking" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divTrackless">
                                        <label for="trackless" class="col-lg-3 text-uppercase fw-bold" >trackless door</label>
                                        <div class="col-lg-4">
                                            <select name="trackless" id="trackless" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFrame">
                                        <label for="frametype" class="col-lg-3 text-uppercase fw-bold" >frame</label>
                                        <div class="col-lg-3" id="divFrameType">
                                            <select name="frametype" id="frametype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-3" id="divFrameColour">
                                            <select name="framecolour" id="framecolour" class="form-select"></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>  
                                        <div class="col-lg-3" id="divCustomFrameColour">
                                            <input type="text" name="customframecolour" id="customframecolour" class="form-control" autocomplete="off" placeholder="Custom ....">
                                            <small class="form-hint">* Custom Colour</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBrace">
                                        <label for="brace" class="col-lg-3 text-uppercase fw-bold" >brace</label>
                                        <div class="col-lg-4" id="">
                                            <select name="brace" id="brace" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-4" id="divBraceLength">
                                            <div class="input-group">
                                                <input type="number" min="1" name="bracelength" id="bracelength" class="form-control  " autocomplete="off"  />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divDualHinges">
                                        <label for="dualhinges" class="col-lg-3 text-uppercase fw-bold" >two hinges</label>
                                        <div class="col-lg-4">
                                            <select name="dualhinges" id="dualhinges" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divInstall">
                                        <label for="install" class="col-lg-3 text-uppercase fw-bold" >Installation</label>
                                        <div class="col-lg-4">
                                            <select name="install" id="install" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFitting">
                                        <label for="fitting" class="col-lg-3 text-uppercase fw-bold" >fitting/options</label>
                                        <div class="col-lg-4">
                                            <select name="fitting" id="fitting" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divRemove">
                                        <label for="remove" class="col-lg-3 text-uppercase fw-bold" >remove product</label>
                                        <div class="col-lg-4">
                                           <select name="remove" id="remove" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divHandle">
                                        <label for="handle" class="col-lg-3 text-uppercase fw-bold" >handle</label>
                                        <div class="col-lg-4">
                                           <select name="handle" id="handle" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divPullCord">
                                        <label for="pullcord" class="col-lg-3 text-uppercase fw-bold" >Pullcord</label>
                                        <div class="col-lg-4">
                                           <select name="pullcord" id="pullcord" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divCutOut">
                                        <label for="cutout" class="col-lg-3 text-uppercase fw-bold" >cut out</label>
                                        <div class="col-lg-8">
                                           <select name="cutout" id="cutout" class="form-select" multiple></select>
                                           <div id="cutoutContainer" class="mt-3"></div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divExtras">
                                        <label for="extras" class="col-lg-3 text-uppercase fw-bold" >Extras</label>
                                        <div class="col-lg-8">
                                           <select name="extras" id="extras" class="form-select" multiple></select>
                                           <div id="extrasContainer" class="mt-3"></div>
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
                            <div class="card-footer text-center">
                                <button type="submit" class="btn btn-primary" id="btnSubmit">Submit</button>
                                <button type="button" class="btn btn-danger" id="btnCancel">
                                    Cancel
                                </button>
                            </div>
                        </form>
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
        let DESIGNIDORI = "9756F316-F324-42FB-9588-874BBFAC50E4";
        let HEADERID = '<%=Session("headerId")%>';
        let ORDERTYPE = '<%= Session("orderType") %>';
        let ITEMACTION = '<%= Session("itemAction") %>';
        let DESIGNID = '<%= Session("designId") %>';
        let ITEMID = '<%= Session("itemId") %>';
        let CUSTOMERID = '<%= Session("CustomerId") %>';
        let LOGINID = '<%= Session("LoginId") %>';
        let ROLENAME = '<%= Session("RoleName") %>';
        let LEVELNAME = '<%= Session("LevelName") %>';
        let MARKUPACCESS = '<%= Session("MarkUpAccess") %>';
        let URIMETHOD = "/Methods/Order/WindowMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Window.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

