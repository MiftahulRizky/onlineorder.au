<%@ Page Title="Veri Shades" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Verishades.aspx.vb" Inherits="Order_Verishades" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblTubeType">shade type</label>
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
                                    </div>

                                    <div class="mb-3 row" id="divFabric">
                                        <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold" id="lblFabricDay">fabric</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabrictype" id="fabrictype" class="form-select "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabriccolour" id="fabriccolour" class="form-select " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divBlindSize">
                                        <label for="blindsize" class="col-lg-3 text-uppercase fw-bold" >blind size</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="blindsize" id="blindsize" class="form-select">
                                                <option value="0"></option>
                                                <option value="1">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divStack">
                                        <label for="stack" class="col-lg-3 text-uppercase fw-bold" >stack confirguration</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="stack" id="stack" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divTrack">
                                        <label for="tracktype" class="col-lg-3 text-uppercase fw-bold" >track</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="tracktype" id="tracktype" class="form-select "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="trackcolour" id="trackcolour" class="form-select " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divWand">
                                        <label for="wandsize" class="col-lg-3 text-uppercase fw-bold">wand</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="wandsize" id="wandsize" class="form-select "></select>
                                            <small class="form-hint">* Size</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="wandcolour" id="wandcolour" class="form-select " ></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12" id="divWandCustomSize">
                                            <input type="number" min="0" name="customsize" id="customsize" class="form-control "  autocomplete="off">
                                            <small class="form-hint">* Custom</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divBracket">
                                        <label for="bracket" class="col-lg-3 text-uppercase fw-bold" >bracket</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="bracket" id="bracket" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-5 row" id="divTape">
                                        <label for="tape" class="col-lg-3 text-uppercase fw-bold" >tape colour</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="tape" id="tape" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divCarriers">
                                        <label for="carrier" class="col-lg-3 text-uppercase fw-bold" >carrier qty</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="carrier" id="carrier" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="carrieroverride" id="carrieroverride" >
                                                    <label class="form-check-label ms-2" for="carrieroverride">Override</label>
                                                </span>
                                            </div>  
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSpacer">
                                        <label for="spacer" class="col-lg-3 text-uppercase fw-bold" >spacer size</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="spacer" id="spacer" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="spaceroverride" id="spaceroverride" >
                                                    <label class="form-check-label ms-2" for="spaceroverride">Override</label>
                                                </span>
                                            </div>  
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSlat">
                                        <label for="slat" class="col-lg-3 text-uppercase fw-bold" >slat size</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="slat" id="slat" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="slatoverride" id="slatoverride" >
                                                    <label class="form-check-label ms-2" for="slatoverride">Override</label>
                                                </span>
                                            </div>  
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divSlatQty">
                                        <label for="slatqty" class="col-lg-3 text-uppercase fw-bold" >slat qty</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="slatqty" id="slatqty" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="slatqtyoverride" id="slatqtyoverride" >
                                                    <label class="form-check-label ms-2" for="slatqtyoverride">Override</label>
                                                </span>
                                            </div>  
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divEndSlat">
                                        <label for="endslats" class="col-lg-3 text-uppercase fw-bold" >end slats</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="endslats" id="endslats" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="endslatsoverride" id="endslatsoverride" >
                                                    <label class="form-check-label ms-2" for="endslatsoverride">Override</label>
                                                </span>
                                            </div>  
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divTotalSlat">
                                        <label for="tape" class="col-lg-3 text-uppercase fw-bold" >total slats</label>
                                        <div class="col-lg-4">
                                            <input type="number" min="0" name="totalslats" id="totalslats" class="form-control "  autocomplete="off" readonly>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divFabricQty">
                                        <label for="fabricqty" class="col-lg-3 text-uppercase fw-bold" >fabric qty</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="0" name="fabricqty" id="fabricqty" class="form-control "  autocomplete="off">
                                                <span class="input-group-text">
                                                    <input class="form-check-input m-0" type="checkbox" name="fabricqtyoverride" id="fabricqtyoverride" >
                                                    <label class="form-check-label ms-2" for="fabricqtyoverride">Override</label>
                                                </span>
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
        let DESIGNIDORI = "28AF4887-5E18-4434-A6A0-08319672D7AA";
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
        let URIMETHOD = "/Methods/Order/VerishadesMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Verishades.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

