<%@ Page Title="Venetian Blinds" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="VenetianBlinds.aspx.vb" Inherits="Order_VenetianBlinds" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">venetian type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divControlType">
                                    <label for="controltype" class="col-lg-3 text-uppercase fw-bold" id="">style</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="controltype" id="controltype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divColourType">
                                    <label for="colourtype" class="col-lg-3 text-uppercase fw-bold" id="">colour</label>
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

                                    <div class="mb-3 row" id="divRoom">
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room / location</label>
                                        <div class="col-lg-4">
                                            <input type="text" name="room" id="room" class="form-control" autocomplete="off">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divSizeType">
                                        <label for="sizetype" class="col-lg-3 text-uppercase fw-bold" >size type</label>
                                        <div class="col-lg-4">
                                            <select name="sizetype" id="sizetype" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divDropFloor">
                                        <label for="dropfloor" class="col-lg-3 text-uppercase fw-bold" >drop to the floor</label>
                                        <div class="col-lg-4">
                                            <select name="dropfloor" id="dropfloor" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMounting">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >mounting</label>
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

                                    <div class="mb-3 row" id="divControl">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold" id="">control</label>
                                        <div class="col-lg-4">
                                            <select name="controlposition" id="controlposition" class="form-select"></select>
                                            <small class="form-hint">* Posistion</small>
                                        </div> 

                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="controllength" id="controllength" class="form-control  " autocomplete="off"/>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoControlLength">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                            <small class="form-hint">* Length</small>
                                        </div>  
                                    </div>

                                    <div id="divControlMock" class="row mb-3">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold" id="">control lift/tilt</label>
                                        <div class="col-lg-4">
                                            <select name="controllift" id="controllift" class="form-select"></select>
                                            <small class="form-hint">* Lift</small>
                                        </div> 
                                        <div class="col-lg-4">
                                            <select name="controltilt" id="controltilt" class="form-select"></select>
                                            <small class="form-hint">* Tilt</small>
                                        </div> 
                                    </div>

                                    <div class="mb-3 row" id="divBracket">
                                        <label for="bracket" class="col-lg-3 text-uppercase fw-bold" >bracket</label>
                                        <div class="col-lg-4">
                                            <select name="bracket" id="bracket" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBottom">
                                        <label for="bottom" class="col-lg-3 text-uppercase fw-bold" >bottom hold down</label>
                                        <div class="col-lg-4">
                                            <select name="bottom" id="bottom" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divHoldBracket">
                                        <label for="holdbracket" class="col-lg-3 text-uppercase fw-bold" >hold down bracket</label>
                                        <div class="col-lg-4">
                                            <select name="holdbracket" id="holdbracket" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="div2on1Headreal">
                                        <label for="twoheadrail" class="col-lg-3 text-uppercase fw-bold" >2 on 1 Headrail</label>
                                        <div class="col-lg-4">
                                            <select name="twoheadrail" id="twoheadrail" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div id="divPelmetDetail">
                                        <div class="mb-3 row">
                                            <label for="notes" class="col-lg-3 text-uppercase fw-bold text-danger">pelmet details</label>
                                        </div>

                                        <div class="mb-3 row" >
                                            <label for="pelmettype" class="col-lg-3 text-uppercase fw-bold" >pelmet</label>
                                            <div class="col-lg-3">
                                                <select name="pelmettype" id="pelmettype" class="form-select"></select>
                                                <small class="form-hint">* Type</small>
                                            </div>  
                                            <div class="col-lg-3" id="divPelmetSize">
                                                <select name="pelmetsize" id="pelmetsize" class="form-select"></select>
                                                <small class="form-hint">* Size</small>
                                            </div>  
                                            <div class="col-lg-3">
                                                <div class="input-group">
                                                    <input type="number" min="1" name="pelmetwidth" id="pelmetwidth" class="form-control  " autocomplete="off"/>
                                                    <button class="btn btn-primary btn-information" type="button" id="btnInfoPelmetWidth">
                                                        <i class="ti ti-info-square-rounded fs-2"></i>
                                                    </button>
                                                </div>
                                                <small class="form-hint">* Width</small> 
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divReturnLength">
                                        <label for="returnleft" class="col-lg-3 text-uppercase fw-bold" >return length</label>
                                        <div class="col-lg-3" id="divReturnLeft">
                                            <div class="input-group">
                                                <input type="number" min="1" name="returnleft" id="returnleft" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Left</small> 
                                        </div>
                                        <div class="col-lg-3" id="divReturnRight">
                                            <div class="input-group">
                                                <input type="number" min="1" name="returnright" id="returnright" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Right</small> 
                                        </div>
                                        <div class="col-lg-2">
                                            <button class="btn btn-primary btn-information" type="button" id="btnInfoReturnLength">
                                                <i class="ti ti-info-square-rounded fs-2"></i>
                                            </button>
                                        </div>
                                    </div>

                                    <div class="mt-6 mb-3 row">
                                        <label for="notes" class="col-lg-3 text-uppercase fw-bold text-danger">cut outs</label>
                                    </div>
                                    <div class="mb-3 row">
                                        <label for="notes" class="col-lg-3 text-uppercase fw-bold text-danger">top</label>
                                    </div>

                                    <div class="mb-3 row" id="">
                                        <label for="toplhswidth" class="col-lg-3 text-uppercase fw-bold" >lhs width - height</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="toplhswidth" id="toplhswidth" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Width</small> 
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="toplhsheight" id="toplhsheight" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Height</small> 
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="">
                                        <label for="toprhswidth" class="col-lg-3 text-uppercase fw-bold" >rhs width - height</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="toprhswidth" id="toprhswidth" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Width</small> 
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="toprhsheight" id="toprhsheight" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Height</small> 
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="notes" class="col-lg-3 text-uppercase fw-bold text-danger">bottom</label>
                                    </div>

                                    <div class="mb-3 row" id="">
                                        <label for="botlhswidth" class="col-lg-3 text-uppercase fw-bold" >lhs width - height</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="botlhswidth" id="botlhswidth" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Width</small> 
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="botlhsheight" id="botlhsheight" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Height</small> 
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="">
                                        <label for="botrhswidth" class="col-lg-3 text-uppercase fw-bold" >rhs width - height</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="botrhswidth" id="botrhswidth" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Width</small> 
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <input type="number" min="1" name="botrhsheight" id="botrhsheight" class="form-control  " autocomplete="off"/>
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Height</small> 
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
                                <p id="pNotes"></p>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>


    <script type="text/javascript">
        let DESIGNIDORI = "86030D51-8409-438A-B1BF-76172ECCBD79";
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
        let URIMETHOD = "/Methods/Order/VenetianMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Venetian.js?<%= DateTime.Now.Ticks %>"></script>


</asp:Content>

