<%@ Page Title="Supply Only" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SupplyOnly.aspx.vb" Inherits="Order_SupplyOnly" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">supply type</label>
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

                                    <div class="mb-3 row" id="divRoom">
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room / location</label>
                                        <div class="col-lg-4">
                                            <input type="text" name="room" id="room" class="form-control" autocomplete="off">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSize">
                                        <label for="size" class="col-lg-3 text-uppercase fw-bold" >Size</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="size" id="size" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row"">
                                        <label for="" class="col-lg-3 text-uppercase fw-bold" id="lblWidth">width</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divWidthInput">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthinput" id="widthinput" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                        </div>                                     
                                        <div class="col-lg-4 col-md-12 col-sm-12" id="divWidthSelect">
                                           <select type="text" name="widthselect" id="widthselect" class="form-select"></select>
                                        </div>                                     
                                    </div>

                                    <div class="mb-3 row" id="divDrop">
                                        <label for="drop" class="col-lg-3 text-uppercase fw-bold" >drop</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="drop" id="drop" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                        </div>                                     
                                    </div>

                                    <div class="mb-3 row" id="divLength">
                                        <label for="length" class="col-lg-3 text-uppercase fw-bold" >length</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="text" min="1" name="length" id="length" class="form-control " autocomplete="off" />
                                                <span class="input-group-text ">lm</span>
                                            </div>
                                        </div>                                     
                                    </div>

                                    <div class="mb-3 row" id="divColour">
                                        <label for="colour" class="col-lg-3 text-uppercase fw-bold" >colour</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="colour" id="colour" class="form-select"></select>
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

                                    <div class="mb-3 row" id="divCutOut">
                                        <label for="cutout" class="col-lg-3 text-uppercase fw-bold" >cut out</label>
                                        <div class="col-lg-4">
                                            <input type="number" min="1" name="cutout" id="cutout" class="form-control" autocomplete="off">
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
        let DESIGNIDORI = "88476979-9A87-447C-9B93-DE76847717D9";
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
        let URIMETHOD = "/Methods/Order/SupplyOnlyMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/SupplyOnly.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

