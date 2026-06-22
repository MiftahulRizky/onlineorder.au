<%@ Page Title="Door" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Door.aspx.vb" Inherits="Order_Door" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                    <label class="form-label" id="lblOrderNumber" >-</label>
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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">door type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="">door product</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="tubetype" id="tubetype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divControlType">
                                    <label for="controltype" class="col-lg-3 text-uppercase fw-bold" id="">mechanism</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="controltype" id="controltype" class="form-select"></select>
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
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >opening</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="width" class="col-lg-3 text-uppercase fw-bold" id="lblWidth">width</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="width" id="width" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint" id="hintWidthTop">* Width</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divWidthMid">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthmid" id="widthmid" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint" id="">* Middle</small>
                                        </div>                                        
                                    </div>

                                    <div class="mb-3 row" id="divWidthBot">
                                        <label for="width" class="col-lg-3 text-uppercase fw-bold" id="lblWidthBot"></label>
                                        <div class="col-lg-4 col-md-12 col-sm-12" >
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthbot" id="widthbot" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint" id="">* Bottom</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="width" class="col-lg-3 text-uppercase fw-bold" >height</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="">
                                            <div class="input-group">
                                                <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Height</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSliding">
                                        <label for="sliding" class="col-lg-3 text-uppercase fw-bold" >Sliding</label>
                                        <div class="col-lg-4">
                                            <select name="sliding" id="sliding" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divStacking">
                                        <label for="stacking" class="col-lg-3 text-uppercase fw-bold" >Stacking</label>
                                        <div class="col-lg-4">
                                            <select name="stacking" id="stacking" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divTrackless">
                                        <label for="trackless" class="col-lg-3 text-uppercase fw-bold" >Trackless Door</label>
                                        <div class="col-lg-4">
                                            <select name="trackless" id="trackless" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" >
                                        <label for="frametype" class="col-lg-3 text-uppercase fw-bold" id="lblFrame">grille</label>
                                        <div class="col-lg-4" id="divFrameType">
                                            <select name="frametype" id="frametype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-4" id="divFrameColour">
                                            <select name="framecolour" id="framecolour" class="form-select"></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divCoating">
                                        <label for="coatingtype" class="col-lg-3 text-uppercase fw-bold" id="">Powder Coating</label>
                                        <div class="col-lg-4" id="divCoatingType">
                                            <select name="coatingtype" id="coatingtype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-4" id="divCoatingColour">
                                            <input type="text" name="coatingcolour" id="coatingcolour" class="form-control" autocomplete="off"">
                                            <small class="form-hint">* Colour</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMesh">
                                        <label for="meshtype" class="col-lg-3 text-uppercase fw-bold" id="lblMesh">mesh</label>
                                        <div class="col-lg-4">
                                            <select name="meshtype" id="meshtype" class="form-select"></select>
                                        </div>  
                                    </div>


                                   <div class="mb-3 row" id="divHandle">
                                        <label for="handleside" class="col-lg-3 text-uppercase fw-bold">handle</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="handleside" id="handleside" class="form-select"></select>
                                            <small class="form-hint">* Side</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divHandleHeight">
                                            <select name="handleheight" id="handleheight" class="form-select"></select>
                                            <small class="form-hint">* Height</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divHandleHeightMM">
                                             <div class="input-group">
                                                <input type="number" min="1" name="handleheightmm" id="handleheightmm" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* In mm</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divHandleNotes">
                                        <label for="handlenotes" class="col-lg-3 text-uppercase fw-bold"></label>
                                        <div class="col-lg-9">
                                            <small class="form-hint text-danger h5" id="lblHandleNotes"></small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divInswing">
                                        <label for="inswing" class="col-lg-3 text-uppercase fw-bold" >inswing hinges</label>
                                        <div class="col-lg-4">
                                            <select name="inswing" id="inswing" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divLockColour">
                                        <label for="lockcolour" class="col-lg-3 text-uppercase fw-bold" >lock colour</label>
                                        <div class="col-lg-4">
                                            <select name="lockcolour" id="lockcolour" class="form-select"></select>
                                        </div>  
                                    </div>
                                    
                                    <div class="mb-3 row" id="divKeyed">
                                        <label for="keyed" class="col-lg-3 text-uppercase fw-bold" >keyed alike</label>
                                        <div class="col-lg-4">
                                            <select name="keyed" id="keyed" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMidrail">
                                        <label for="midrail" class="col-lg-3 text-uppercase fw-bold" >Mid Rail</label>
                                        <div class="col-lg-4">
                                            <select name="midrail" id="midrail" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-4" id="divMidrailLength">
                                            <div class="input-group">
                                                <input type="number" min="1" name="midraillength" id="midraillength" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                    </div>

                                   

                                    <div class="mb-3 row" id="divBugseal">
                                        <label for="bugseal" class="col-lg-3 text-uppercase fw-bold" >bug seals</label>
                                        <div class="col-lg-4">
                                            <select name="bugseal" id="bugseal" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divCloser">
                                        <label for="closer" class="col-lg-3 text-uppercase fw-bold" id="lblCloser">closer</label>
                                        <div class="col-lg-4">
                                            <select name="closer" id="closer" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divInstall">
                                        <label for="install" class="col-lg-3 text-uppercase fw-bold" >installation</label>
                                        <div class="col-lg-4">
                                            <select name="install" id="install" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFixing">
                                        <label for="fixing" class="col-lg-3 text-uppercase fw-bold" >fixing</label>
                                        <div class="col-lg-4">
                                            <select name="fixing" id="fixing" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFitted">
                                        <label for="fitted" class="col-lg-3 text-uppercase fw-bold" >fitted</label>
                                        <div class="col-lg-4">
                                            <select name="fitted" id="fitted" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divRemove">
                                        <label for="remove" class="col-lg-3 text-uppercase fw-bold" >remove product</label>
                                        <div class="col-lg-4">
                                            <select name="remove" id="remove" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divPetDor">
                                        <label for="petdoortype" class="col-lg-3 text-uppercase fw-bold">Pet Dor</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="petdoortype" id="petdoortype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="petdoorposition" id="petdoorposition" class="form-select"></select>
                                            <small class="form-hint">* Posotion</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divPetDorPositionW">
                                            <input type="text" name="petdoorpositionw" id="petdoorpositionw" class="form-control " autocomplete="off" />
                                            <small class="form-hint">* Write</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divHalf">
                                        <label for="half" class="col-lg-3 text-uppercase fw-bold" >half panel</label>
                                        <div class="col-lg-4">
                                            <select name="half" id="half" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divInterlock">
                                        <label for="interlock" class="col-lg-3 text-uppercase fw-bold" id="lblInterlock">interlocks and options</label>
                                        <div class="col-lg-4">
                                            <select name="interlock" id="interlock" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divExtras">
                                        <label for="extras" class="col-lg-3 text-uppercase fw-bold" >extras</label>
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
        let DESIGNIDORI = "397EB210-E264-49EC-ABF4-DA39B6C018F3";
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
        let URIMETHOD = "/Methods/Order/DoorMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Door.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

