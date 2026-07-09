<%@ Page Title="Vertical Blinds" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="VerticalBlinds.aspx.vb" Inherits="Order_VerticalBlinds" MaintainScrollPositionOnPostback="true" Debug="true" %>
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
                <div class="col-lg-8 col-md-8 col-sm-8">
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

                <div class="col-8 col-lg-8 col-md-12">
                    <div class="card">
                        <form action="javascript:void(0)" method="post" id="formSubmit">
                            <div class="card-header d-flex justify-content-between">
                                <h3 class="card-title" id="cardTitle"></h3>
                            </div>
                            <div class="card-body">
                                
                                <div class="mb-3 row">
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">vertical type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblTubeType">vertical style</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="tubetype" id="tubetype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divControlType">
                                    <label for="controltype" class="col-lg-3 text-uppercase fw-bold">control type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="controltype" id="controltype" class="form-select"></select>
                                    </div>  
                                </div>


                                <div  id="divFormDetail">
                                    <hr/>

                                    <div class="mb-3 row" >
                                        <label for="qty" class="col-lg-3 text-uppercase fw-bold" >quantity</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" value="1" name="qty" id="qty" class="form-control">
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoQty">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" >
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room to install</label>
                                        <div class="col-lg-4">
                                            <input type="text" name="room" id="room" class="form-control" autocomplete="off">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMounting">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >mounting</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-select">
                                                <option value=""></option>
                                                <option value="Face Fit">FACE FIT</option>
                                                <option value="Reveal Fit">REVEAL FIT</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="width" class="col-lg-3 text-uppercase fw-bold" id="lblWd">width x drop</label>
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
                                        <div class="col-lg-2 col-md-12 col-sm-12" id="divInfoWD">
                                            <button class="btn btn-primary btn-information" type="button" id="btnInfoWD">
                                                <i class="ti ti-info-square-rounded fs-2"></i>
                                            </button>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSlatSize">
                                        <label for="slatsize" class="col-lg-3 text-uppercase fw-bold">slat size</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="slatsize" id="slatsize" class="form-select"></select>
                                        </div>  
                                    </div>
                                    <div class="mb-3 row" id="divSlatQty">
                                        <label for="slatqty" class="col-lg-3 text-uppercase fw-bold">slat qty</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="slatqty" id="slatqty" class="form-control  " autocomplete="off" />
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoSlatQty">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFabric">
                                        <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">fabric type x slat x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabrictype" id="fabrictype" class="form-select "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-2 col-md-12 col-sm-12">
                                            <select name="fabriclength" id="fabriclength" class="form-select "></select>
                                            <small class="form-hint">* Slat</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="fabriccolour" id="fabriccolour" class="form-select " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divTrackColour">
                                        <label for="trackcolour" class="col-lg-3 text-uppercase fw-bold">track colour</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="trackcolour" id="trackcolour" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divStackPosition">
                                        <label for="stackposition" class="col-lg-3 text-uppercase fw-bold">stack position</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="stackposition" id="stackposition" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divControlPosition">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold" id="lblControlPosition">control position</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="controlposition" id="controlposition" class="form-select">
                                                <option value=""></option>
                                                <option value="Left">LEFT</option>
                                                <option value="Right">RIGHT</option>
                                                <option value="Twin Wand">TWIN WAND</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divChain">
                                        <label for="chaincolour" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">chain colour x length</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="chaincolour" id="chaincolour" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="chainlength" id="chainlength" class="form-control ">
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divWand">
                                        <label for="wandlength" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">wand Length x colour</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="wandlength" id="wandlength" class="form-select "></select>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="wandcolour" id="wandcolour" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divWandCustomLength">
                                            <div class="input-group">
                                                <input type="number" min="1" name="wandcustomlength" id="wandcustomlength" class="form-control ">
                                                <!-- <span class="input-group-text ">mm</span> -->
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoCustomLength">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                            <small class="form-hint">*Custom  Length</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divBrackets">
                                        <label for="bracket" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">bracket type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="bracket" id="bracket" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="bracketcolour" id="bracketcolour" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divHangerType">
                                        <label for="hangertype" class="col-lg-3 text-uppercase fw-bold">hanger type</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="hangertype" id="hangertype" class="form-select"></select>
                                        </div>  
                                    </div>
                                    <div class="mb-3 row" id="divBottom">
                                        <label for="bottom" class="col-lg-3 text-uppercase fw-bold">bottom</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="bottom" id="bottom" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divInsertInTrack">
                                        <label for="inserttrack" class="col-lg-3 text-uppercase fw-bold" >insert in track</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="inserttrack" id="inserttrack" class="form-select">
                                                <option value=""></option>
                                                <option value="1">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSloper">
                                        <label for="sloper" class="col-lg-3 text-uppercase fw-bold" >sloper</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="sloper" id="sloper" class="form-select">
                                                <option value=""></option>
                                                <option value="1">YES</option>
                                            </select>
                                             <small class="form-hint">* Blades will be Tilt Only - Track supplied First</small>
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
                                <button type="submit" class="btn btn-primary" id="btnSubmit">Save Changes</button>
                                <button type="button" class="btn btn-danger" id="btnCancel">
                                    Cancel
                                </button>
                            </div>
                        </form>
                    </div>
                </div>

                <div class="col-lg-4 col-md-12 col-sm-12">
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
        let DESIGNIDORI = "B556E35C-CEAC-40F8-A6CF-156601BD57DA";
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
        let URIMETHOD = "/Methods/Order/VerticalBlindMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/VerticalBlinds.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

