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
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblTubeType">Slat Type</label>
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

                                <div class="mb-3 row" id="divColourType">
                                    <label for="colourtype" class="col-lg-3 text-uppercase fw-bold">headrail colour</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="colourtype" id="colourtype" class="form-select"></select>
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
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room to install</label>
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

                                    <div class="mb-3 row" id="divFabricInsert">
                                        <label for="fabricinsert" class="col-lg-3 text-uppercase fw-bold">fabric insert</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="fabricinsert" id="fabricinsert" class="form-select"></select>
                                                <button class="btn btn-info btn-information" type="button" id="btnInfoFabricInsert">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFabric">
                                        <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">fabric type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabrictype" id="fabrictype" class="form-select "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabriccolour" id="fabriccolour" class="form-select " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divBladeQty">
                                        <label for="bladeqty" class="col-lg-3 text-uppercase fw-bold" >blade quantity</label>
                                        <div class="col-lg-4">
                                            <input type="number" name="bladeqty" id="bladeqty" class="form-control">
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
                                            <select type="text" name="controlposition" id="controlposition" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divChain">
                                        <label for="chaincolour" class="col-lg-3 text-uppercase fw-bold" id="">chain colour x length</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="chaincolour" id="chaincolour" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <div class="input-group">
                                                     <select name="chainlength" id="chainlength" class="form-select "></select>
                                                    <button class="btn btn-info btn-information" type="button" id="btnInfoCustomChainLength">
                                                        <i class="ti ti-info-square-rounded fs-2"></i>
                                                    </button>
                                                </div>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divCustomChainLength">
                                            <div class="input-group">
                                                <input type="number" min="1" name="customchainlength" id="customchainlength" class="form-control ">
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">*Custom  Length</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divWand">
                                        <label for="wandlength" class="col-lg-3 text-uppercase fw-bold" id="">wand colour x length</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="wandcolour" id="wandcolour" class="form-select "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <select name="wandlength" id="wandlength" class="form-select "></select>
                                                <button class="btn btn-info btn-information" type="button" id="btnInfoCustomWandLength">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divWandCustomLength">
                                            <div class="input-group">
                                                <input type="number" min="1" name="customwandlength" id="customwandlength" class="form-control ">
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">*Custom  Length</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divBottomJoining">
                                        <label for="bottomjoining" class="col-lg-3 text-uppercase fw-bold">bottom joining</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="bottomjoining" id="bottomjoining" class="form-select"></select>
                                                <button class="btn btn-info btn-information" type="button" id="btnInfoBottomJoining">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divExtBracket">
                                        <label for="extbracket" class="col-lg-3 text-uppercase fw-bold">extension bracket</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="extbracket" id="extbracket" class="form-select"></select>
                                                <button class="btn btn-info btn-information" type="button" id="btnInfoExtBracket">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSloping">
                                        <label for="sloping" class="col-lg-3 text-uppercase fw-bold">sloping</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="sloping" id="sloping" class="form-select"></select>
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

                                <p class="text-uppercase fw-bold">Limitations</p> <br>
                                <p> 
                                    - Minimum width : 300mm <br>
                                    - Maximum width for 89mm Slat : 5511mm <br>
                                    - Maximum width for 100mm Slat : 5477mm <br>
                                    - Maximum width for 127mm Slat : 5477mm <br>
                                    - Maximum drop : 3050mm
                                </p>
                                <div class="col-2 col-lg-2 col-sm-2">
                                    <button type="button" class="btn btn-info" id="btnInfoGallery">
                                        <i class="ti ti-info-square-rounded fs-2 me-1"></i> Detail Gallery
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <script type="text/javascript">
        let DESIGNIDORI = "B71CD2B3-E64A-4D47-B974-51CBEBD2C578";
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
        let URIMETHOD = "/Methods/Order/Global/VerticalBlindMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Global/VerticalBlinds.js?v=1.0.47"></script>
</asp:Content>

