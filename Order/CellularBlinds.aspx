<%@ Page Title="Cellular Blinds" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CellularBlinds.aspx.vb" Inherits="Order_CellularBlinds" MaintainScrollPositionOnPostback="true" Debug="true"%>

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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">cellular type</label>
                                    <div class="col-lg-5">
                                        <select type="text" name="blindtype" id="blindtype" class="form-control"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divBracketType">
                                    <label for="brackettype" class="col-lg-3 text-uppercase fw-bold" id="lblBracketType">cell type</label>
                                    <div class="col-lg-5">
                                        <select type="text" name="brackettype" id="brackettype" class="form-control "></select>
                                    </div>  
                                </div>
            
                                <div class="mb-3 row" id="divControlType">
                                    <label for="controltype" class="col-lg-3 text-uppercase fw-bold" id="lblControlType">control type</label>
                                    <div class="col-lg-5">
                                        <select type="text" name="controltype" id="controltype" class="form-control "></select>
                                    </div>  
                                </div>

                                <div  id="divFormDetail">
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

                                    <div class="mb-3 row" id="divSizeType">
                                        <label for="sizetype" class="col-lg-3 text-uppercase fw-bold" >size type</label>
                                        <div class="col-lg-4">
                                            <select name="sizetype" id="sizetype" class="form-select"></select>
                                        </div>  
                                    </div>
    
                                    <div class="mb-3 row">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold">Mounting</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-control "></select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divDropFloor">
                                        <label for="dropfloor" class="col-lg-3 text-uppercase fw-bold" >drop to the floor</label>
                                        <div class="col-lg-4">
                                            <select name="dropfloor" id="dropfloor" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="width" class="col-lg-3 text-uppercase fw-bold">width x drop</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" min="1" name="width" id="width" class="form-control " autocomplete="off" placeholder="Width ...." />
                                            <span class="input-group-text ">mm</span>
                                        </div>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Drop ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divFabricDay">
                                        <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">fabric type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divFabricDayType">
                                            <select name="fabrictype" id="fabrictype" class="form-control "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divFabricDayColour">
                                            <select name="fabriccolour" id="fabriccolour" class="form-control " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divFabricNight">
                                        <label for="fabrictype2" class="col-lg-3 text-uppercase fw-bold" id="lblFabricNight">fabric type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabrictype2" id="fabrictype2" class="form-control "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabriccolour2" id="fabriccolour2" class="form-control "></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divControlSystem">
                                        <label for="controlsystem" class="col-lg-3 text-uppercase fw-bold">control type</label>
                                        <div class="col-lg-6">
                                            <select name="controlsystem" id="controlsystem" class="form-control" multiple ></select>   
                                        </div>
                                    </div>
    
                                    <div class="mb-3 row" id="divCordType">
                                        <label for="cordtype" class="col-lg-3 text-uppercase fw-bold">cord type</label>
                                        <div class="col-lg-4">
                                            <select name="cordtype" id="cordtype" class="form-control "></select>   
                                        </div>
                                    </div>
    
                                    <div class="mb-3 row">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold">control side</label>
                                        <div class="col-lg-4">
                                            <select name="controlposition" id="controlposition" class="form-control "></select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="qty" class="col-lg-3 text-uppercase fw-bold">chain length</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="chainlength" id="chainlength" class="form-control  " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="divMotor">
                                        <div class="mb-3 row">
                                            <label for="motortype" class="col-lg-3 text-uppercase fw-bold">motor type</label>
                                            <div class="col-lg-4">
                                                <select name="motortype" id="motortype" class="form-control "></select>   
                                            </div>
                                        </div>
                                        <div class="mb-3 row">
                                            <label for="motorextra" class="col-lg-3 text-uppercase fw-bold">motor extra</label>
                                            <div class="col-lg-4">
                                                <select name="motorextra" id="motorextra" class="form-control "></select>   
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="holddown" class="col-lg-3 text-uppercase fw-bold">Hold Down Bracket</label>
                                        <div class="col-lg-4">
                                            <select name="holddown" id="holddown" class="form-control "></select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="cutout" class="col-lg-3 text-uppercase fw-bold">cut out</label>
                                        <div class="col-lg-4">
                                            <select name="cutout" id="cutout" class="form-control "></select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divAdditional">
                                        <label for="additional" class="col-lg-3 text-uppercase fw-bold">Additional</label>
                                        <div class="col-lg-4">
                                            <select name="additional" id="additional" class="form-control "></select>
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
        let DESIGNIDORI = "35905E47-9B37-485B-A3FD-281BE4E3A94E";
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
        let URIMETHOD = "/Methods/Order/CellularBlindMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/CellularBlinds.js?<%= DateTime.Now.Ticks %>"></script>

</asp:Content>

