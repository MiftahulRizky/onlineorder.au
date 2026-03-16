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

                                <div class="mb-3 row" id="divColourType">
                                    <label for="colourtype" class="col-lg-3 text-uppercase fw-bold" id="lblColourType">window product</label>
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
                                                <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Drop ...." />
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

                                    <div class="mb-3 row" id="divFrameColour">
                                        <label for="framecolour" class="col-lg-3 text-uppercase fw-bold" >frame colour</label>
                                        <div class="col-lg-4">
                                            <select name="framecolour" id="framecolour" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBrace">
                                        <label for="brace" class="col-lg-3 text-uppercase fw-bold" >brace</label>
                                        <div class="col-lg-4">
                                            <select name="brace" id="brace" class="form-select">
                                                <option value=""></option>
                                                <option value="Centre of Horizontal">CENTRE OF HORIZONTAL</option>
                                                <option value="Centre of Vertical">CENTRE OF VERTICAL</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divAngle">
                                        <label for="angletype" class="col-lg-3 text-uppercase fw-bold">angle type x Length x qty</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="angletype" id="angletype" class="form-select">
                                                <option value=""></option>
                                                <option value="12x12mm">12X12MM</option>
                                                <option value="12x20mm">12X20MM</option>
                                                <option value="12x25mm">12X25MM</option>
                                                <option value="20x20mm">20X20MM</option>
                                                <option value="20x25mm">20X25MM</option>
                                                <option value="20x40mm">20X40MM</option>
                                                <option value="25x50mm">25X50MM</option>
                                            </select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="anglelength" id="anglelength" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <input type="number" min="1" name="angleqty" id="angleqty" class="form-control  " autocomplete="off"/>
                                            <small class="form-hint">* Qty</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divPortHole">
                                        <label for="porthole" class="col-lg-3 text-uppercase fw-bold" >screen port hole</label>
                                        <div class="col-lg-4">
                                            <select name="porthole" id="porthole" class="form-select">
                                                <option value=""></option>
                                                <option value="Supply Loose">SUPPLY LOOSE</option>
                                                <option value="Fitted (Diagram)">FITTED (DIAGRAM)</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divPortHole">
                                        <label for="plungerpin" class="col-lg-3 text-uppercase fw-bold" >plunger pin</label>
                                        <div class="col-lg-4">
                                            <select name="plungerpin" id="plungerpin" class="form-select">
                                                <option value=""></option>
                                                <option value="Metal Loose (4)">METAL LOOSE (4)</option>
                                                <option value="Metal Loose (6)">METAL LOOSE (6)</option>
                                                <option value="Plain Loose (4)">PLAIN LOOSE (4)</option>
                                                <option value="Plain Loose (6)">PLAIN LOOSE (6)</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSwivalColour">
                                        <label for="swivelcolour" class="col-lg-3 text-uppercase fw-bold" >swivel clip colour</label>
                                        <div class="col-lg-4">
                                            <select name="swivelcolour" id="swivelcolour" class="form-select">
                                                <option value=""></option>
                                                <option value="Black">BLACK</option>
                                                <option value="White">WHITE</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSwivalQty">
                                        <label for="swivelqty" class="col-lg-3 text-uppercase fw-bold">swivel clip qty</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <span class="input-group-text ">1.6MM</span>
                                                <input type="number" min="1" name="swivelqty" id="swivelqty" class="form-control " autocomplete="off" />
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <span class="input-group-text ">11MM</span>
                                                <input type="number" min="1" name="swivelqty" id="swivelqty" class="form-control  " autocomplete="off"/>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSpringQty">
                                        <label for="springqty" class="col-lg-3 text-uppercase fw-bold" >spring clip qty</label>
                                        <div class="col-lg-4">
                                           <input type="number" id="springqty" name="springqty" min="1" class="form-control ">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divTopPlasticQty">
                                        <label for="topplasticqty" class="col-lg-3 text-uppercase fw-bold" >spring clip qty</label>
                                        <div class="col-lg-4">
                                           <input type="number" id="topplasticqty" name="topplasticqty" min="1" class="form-control ">
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

