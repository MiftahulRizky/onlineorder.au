<%@ Page Title="Roller Blinds" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="RollerBlinds.aspx.vb" Inherits="Order_RollerBlinds" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                <label class="card-title" id="lblBlindNo"></label>
                                <label class="card-title" id="lblUniqueId"></label>
                            </div>
                            <div class="card-body">
                                
                                <div class="mb-3 row">
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">roller type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divBracketType">
                                    <label for="brackettype" class="col-lg-3 text-uppercase fw-bold" id="lblBracketType">bracket type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="brackettype" id="brackettype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblTubeType">mechanism type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="tubetype" id="tubetype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divControlType">
                                    <label for="controltype" class="col-lg-3 text-uppercase fw-bold">control type</label>
                                    <div class="col-lg-4">
                                        <div class="input-group">
                                            <select type="text" name="controltype" id="controltype" class="form-select"></select>
                                            <button class="btn btn-primary btn-information" type="button" id="btnInfoControlType">
                                                <i class="ti ti-info-square-rounded fs-2"></i>
                                            </button>
                                        </div>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divColourType">
                                    <label for="colourtype" class="col-lg-3 text-uppercase fw-bold" id="lblColourType">control colour</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="colourtype" id="colourtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div  id="divFormDetail">
                                    <hr/>

                                    <div class="mb-3 row"  id="divAttention">
                                        <div class="col-lg-12">
                                            <div class="alert alert-danger" role="alert">
                                                <div class="d-flex">
                                                    <div class="me-1">
                                                        <i class="ti ti-alert-square-rounded fs-2"></i>
                                                    </div>
                                                    <div>
                                                        <h4 class="alert-title">ATTENTION !</h4>
                                                        <div class="text-secondary">
                                                            <span id="lblNextDesc">Lorem ipsum dolor sit amet.</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

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

                                    <div class="mb-3 row" id="divSizeType">
                                        <label for="sizetype" class="col-lg-3 text-uppercase fw-bold" >size type</label>
                                        <div class="col-lg-4">
                                            <select name="sizetype" id="sizetype" class="form-select"></select>
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
                                            <small class="form-hint">* Width</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Drop ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Drop</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
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

                                    <div class="mb-3 row" id="divMotorStyle">
                                        <label for="motorstyle" class="col-lg-3 text-uppercase fw-bold">motor style</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="motorstyle" id="motorstyle" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoMotorStyle">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMotorRemote">
                                        <label for="motorremote" class="col-lg-3 text-uppercase fw-bold">motor remote</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="motorremote" id="motorremote" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoMotorRemote">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMotorBattery">
                                        <label for="externalbattery" class="col-lg-3 text-uppercase fw-bold">external battery</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="externalbattery" id="externalbattery" class="form-select">
                                                <!-- <option value=""></option> -->
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMotorCharger">
                                        <label for="charger" class="col-lg-3 text-uppercase fw-bold">charger</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="charger" id="charger" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divCableExitPoint">
                                        <label for="cableexitpoint" class="col-lg-3 text-uppercase fw-bold">cable exit point</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="cableexitpoint" id="cableexitpoint" class="form-select">
                                                <option value=""></option>
                                                <option value="Side">SIDE</option>
                                                <option value="Top">TOP</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divConnector">
                                        <label for="connector" class="col-lg-3 text-uppercase fw-bold">flush connect</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="connector" id="connector" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                            <small class="form-hint">* Flush Connector Housing</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divRoll">
                                        <label for="roll" class="col-lg-3 text-uppercase fw-bold">roll direction</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="roll" id="roll" class="form-select">
                                                <option value=""></option>
                                                <option value="Reverse Roll">REVERSE ROLL</option>
                                                <option value="Standard">STANDARD</option>
                                            </select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divControlPosition">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold" id="lblControlPosition">control position</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="controlposition" id="controlposition" class="form-select">
                                                    <option value=""></option>
                                                    <option value="Left">LEFT</option>
                                                    <option value="Right">RIGHT</option>
                                                </select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoControlPosition">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
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

                                    <div class="mb-3 row" id="divTrim">
                                        <label for="trim" class="col-lg-3 text-uppercase fw-bold" >trim</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="trim" id="trim" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoTrim">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBottomRail">
                                        <label for="railtype" class="col-lg-3 text-uppercase fw-bold">bottom rail type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="railtype" id="railtype" class="form-select "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <select name="railcolour" id="railcolour" class="form-select "></select>
                                            </div>
                                            <small class="form-hint">* Colour</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divTubeSize">
                                        <label for="tubesize" class="col-lg-3 text-uppercase fw-bold" >tube size</label>
                                        <div class="col-lg-4">
                                            <div class="input-group">
                                                <select type="text" name="tubesize" id="tubesize" class="form-select"></select>
                                                <button class="btn btn-primary btn-information" type="button" id="btnInfoTubeSize">
                                                    <i class="ti ti-info-square-rounded fs-2"></i>
                                                </button>
                                            </div>
                                        </div>  
                                    </div>

                                    <div class="mb-1 mt-6 row" id="divAdditional">
                                        <p class="text-danger fw-bold text-uppercase text-underline">
                                            additional
                                        </p>
                                    </div>

                                    <div class="mb-3 row" id="divChildSafe">
                                        <label for="childsafe" class="col-lg-3 text-uppercase fw-bold" >childsafe</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="childsafe" id="childsafe" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divAccessory">
                                        <label for="accessory" class="col-lg-3 text-uppercase fw-bold" >accessory</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="accessory" id="accessory" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divExtras">
                                        <label for="extras" class="col-lg-3 text-uppercase fw-bold" >extras</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="extras" id="extras" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBracketCover">
                                        <label for="bracketcovers" class="col-lg-3 text-uppercase fw-bold" >bracket cover</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="bracketcovers" id="bracketcovers" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                            <small class="form-hint">* Cover</small>
                                        </div>  
                                        <div class="col-lg-4" id="divBracketCoverColour">
                                            <select type="text" name="bracketcovercolours" id="bracketcovercolours" class="form-select"></select>
                                            <small class="form-hint">* Colour</small>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBracketExt">
                                        <label for="bracketext" class="col-lg-3 text-uppercase fw-bold" >bracket extension</label>
                                        <div class="col-lg-4">
                                            <select type="text" name="bracketext" id="bracketext" class="form-select">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
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
        let DESIGNIDORI = "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2";
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
        let URIMETHOD = "/Methods/Order/RollerBlindMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/RollerBlinds.js?v=1.0.5"></script>
</asp:Content>
