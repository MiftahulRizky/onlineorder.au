<%@ Page Title="Curtains" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Curtains.aspx.vb" Inherits="Order_Curtains" MaintainScrollPositionOnPostback="true" Debug="true" %>
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

            <div class="row mb-3">

                <div class="col-7 col-lg-7 col-md-12">
                    <div class="card">
                        <form action="javascript:void(0)" method="post" id="formSubmit">
                            <div class="card-header d-flex justify-content-between">
                                <h3 class="card-title" id="cardTitle"></h3>
                            </div>
                            <div class="card-body">
                                
                                <div class="mb-3 row">
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">curtain type</label>
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
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >Fitting</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-select"></select>
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

                                    <div class="mb-3 row" id="divCurtainHeading">
                                        <label for="curtainheading" class="col-lg-3 text-uppercase fw-bold">curtain heading</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="curtainheading" id="curtainheading" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoHeading">
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

                                    <div class="mb-3 row" id="divTrack">
                                        <label for="tracktype" class="col-lg-3 text-uppercase fw-bold">track</label>
                                        <div class="col-lg-3">
                                            <select type="text" name="tracktype" id="tracktype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>  
                                        <div class="col-lg-3">
                                            <select type="text" name="trackcolour" id="trackcolour" class="form-select"></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-3">
                                            <select type="text" name="trackdraw" id="trackdraw" class="form-select"></select>
                                            <small class="form-hint">* Draw</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divStackPosition">
                                        <label for="stackposition" class="col-lg-3 text-uppercase fw-bold">stack position</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="stackposition" id="stackposition" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divReturnLength">
                                        <label for="returnleft" class="col-lg-3 text-uppercase fw-bold" id="lblWd">return length</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="returnleft" id="returnleft" class="form-control " autocomplete="off" placeholder="Left ...."  />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Left</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divDrop">
                                            <div class="input-group">
                                                <input type="number" min="1" name="returnright" id="returnright" class="form-control  " autocomplete="off" placeholder="Right ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Right</small>
                                        </div>
                                    </div>
                                   
                                    <div class="mb-3 row" id="divBottom">
                                        <label for="bottom" class="col-lg-3 text-uppercase fw-bold">bottom hem</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="bottom" id="bottom" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divTie">
                                        <label for="tie" class="col-lg-3 text-uppercase fw-bold" >tie back req</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="tie" id="tie" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoTie">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
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
        let DESIGNIDORI = "C1DBF139-2F7C-49F7-A71D-CB7E7667CC96";
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
        let URIMETHOD = "/Methods/Order/CurtainMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Curtains.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

