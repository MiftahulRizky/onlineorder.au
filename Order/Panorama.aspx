<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Panorama.aspx.vb" Inherits="Order_Panorama" MasterPageFile="~/Site.Master" Title="Panorama PVC Shutters" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                        <span>ORDER</span>
                    </div>
                    <h2 class="page-title" id="pageTitle"></h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container container-slim py-4" id="divLoader">
            <div class="text-center">
                <div class="row mb-xxl-7">
                    <br />
                </div>
                <div class="row mb-xxl-8">
                    <a href="." class="navbar-brand navbar-brand-autodark"><img runat="server" src="~/Content/static/ShutterLogo.png"></a>
                </div>
                <div class="text-secondary mb-3">PREPARING DATA</div>
                <div class="progress progress-sm">
                    <div class="progress-bar progress-bar-indeterminate"></div>
                </div>
            </div>
        </div>
        <div class="container-xl" id="divOrder" style="display:none;">
            <div class="row mb-3">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Order #</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="orderid"></span>
                                    </div>
                                </div>
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Order Number</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="orderno"></span>
                                    </div>
                                </div>
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Customer Name</div>
                                    <div class="datagrid-content datagridContent">
                                        <span id="ordername"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="cardTitle"></h3>
                        </div>
                        <div class="card-body">
                            <div class="mb-2 row">
                                <label class="col-lg-3 col-form-label required">TYPE</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <select class="form-select" id="blindtype" style="font-weight:bold;"></select>
                                </div>
                            </div>

                            <div class="mb-2 row">
                                <label class="col-lg-3 col-form-label required">COLOUR</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <select class="form-select" id="colourtype" style="font-weight:bold;"></select>
                                </div>
                            </div>

                            <div id="divDetail">
                                <hr />
                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label required">QUANTITY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <input type="number" id="qty" class="form-control" autocomplete="off" placeholder="Quantity" value="1" />
                                    </div>
                                </div>

                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label required">ROOM / LOCATION</label>
                                    <div class="col-lg-6 col-md-12 col-sm-12">
                                        <input type="text" id="room" class="form-control" autocomplete="off" placeholder="Room / Location" />
                                    </div>
                                </div>

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label required">MOUNTING</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select class="form-select" id="mounting"></select>
                                    </div>
                                </div>

                                <div class="mb-5 row" id="divSemiInsideMount">
                                    <label class="col-lg-3 col-form-label">SEMI INSIDE MOUNT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="semiinsidemount">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-5 row">
                                    <label class="col-lg-3 col-form-label required">WIDTH x HEIGHT</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="width" class="form-control" autocomplete="off" placeholder="Width ...." />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Width</small>
                                    </div>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="drop" class="form-control" autocomplete="off" placeholder="Height ...." />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height</small>
                                    </div>
                                </div>

                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label required">LOUVRE SIZE</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select class="form-select" id="louvresize">
                                            <option value=""></option>
                                            <option value="63">63MM</option>
                                            <option value="89">89MM</option>
                                            <option value="114">114MM</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divLouvrePosition">
                                    <label class="col-lg-3 col-form-label required">LOUVRE POSITION</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select class="form-select" id="louvreposition">
                                            <option value=""></option>
                                            <option value="Open">OPEN</option>
                                            <option value="Closed">CLOSED</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row">
                                    <label class="col-lg-3 col-form-label">MIDRAIL HEIGHT</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="midrailheight1" class="form-control" autocomplete="off" placeholder="Height 1" />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 1</small>
                                    </div>
                                    <div class="col-lg-3 col-md-12 col-sm-12" id="divMidrailHeight2">
                                        <div class="input-group">
                                            <input type="number" id="midrailheight2" class="form-control" autocomplete="off" placeholder="Height 2" />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 2</small>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divMidrailCritical">
                                    <label class="col-lg-3 col-form-label">CRITICAL MIDRAIL</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select class="form-select" id="midrailcritical"></select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divPanelQty">
                                    <label class="col-lg-3 col-form-label required">PANEL QTY</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="panelqty">
                                            <option value=""></option>
                                            <option value="1">1</option>
                                            <option value="2">2</option>
                                            <option value="3">3</option>
                                            <option value="4">4</option>
                                            <option value="5">5</option>
                                            <option value="6">6</option>
                                            <option value="7">7</option>
                                            <option value="8">8</option>
                                            <option value="9">9</option>
                                            <option value="10">10</option>
                                            <option value="11">11</option>
                                            <option value="12">12</option>
                                            <option value="13">13</option>
                                            <option value="14">14</option>
                                            <option value="15">15</option>
                                            <option value="16">16</option>
                                            <option value="17">17</option>
                                            <option value="18">18</option>
                                            <option value="19">19</option>
                                            <option value="20">20</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 mt-6 row" id="divJoinedPanels">
                                     <label class="col-lg-3 col-form-label">CO-JOINED PANELS</label>
                                     <div class="col-lg-2 col-md-12 col-sm-12">
                                         <select class="form-select" id="joinedpanels">
                                             <option value=""></option>
                                             <option value="Yes">YES</option>
                                         </select>
                                     </div>
                                 </div>

                                <div class="mb-2 mt-5 row" id="divHingeColour">
                                    <label class="col-lg-3 col-form-label required">HINGE COLOUR</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select class="form-select" id="hingecolour">
                                            <option value=""></option>
                                            <option value="Default">DEFAULT</option>
                                            <option value="Off White">OFF WHITE</option>
                                            <option value="Stainless Steel">STAINLESS STEEL</option>
                                            <option value="White">WHITE</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-3 mt-5 row" id="divCustomHeaderLength">
                                    <label class="col-lg-3 col-form-label">CUSTOM HEADER</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="customheaderlength" class="form-control" autocomplete="off" placeholder="Length ...." />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Length</small>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divLayoutCode">
                                    <label class="col-lg-3 col-form-label required">LAYOUT CODE</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select class="form-select" id="layoutcode"></select>
                                    </div>

                                    <div class="col-lg-4 col-md-12 col-sm-12" id="divLayoutCodeCustom">
                                        <input type="text" id="layoutcodecustom" class="form-control" autocomplete="off" placeholder="Custom ...." />
                                        <small class="form-hint">* Custom Layout Code (Layout Other)</small>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divFrameType">
                                    <label class="col-lg-3 col-form-label required">FRAME TYPE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select class="form-select" id="frametype"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divFrameLeft">
                                    <label class="col-lg-3 col-form-label required">LEFT FRAME</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <select class="form-select" id="frameleft"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divFrameRight">
                                    <label class="col-lg-3 col-form-label required">RIGHT FRAME</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <select class="form-select" id="frameright"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divFrameTop">
                                    <label class="col-lg-3 col-form-label required">TOP FRAME</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <select class="form-select" id="frametop"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divFrameBottom">
                                    <label class="col-lg-3 col-form-label required">BOTTOM FRAME</label>
                                    <div class="col-lg-5 col-md-12 col-sm-12">
                                        <select class="form-select" id="framebottom"></select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divBottomTrackType">
                                    <label class="col-lg-3 col-form-label required">BOTTOM TRACK TYPE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <select class="form-select" id="bottomtracktype"></select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divBottomTrackRecess">
                                    <label class="col-lg-3 col-form-label">BOTTOM TRACK RECESS</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="bottomtrackrecess">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divBuildout">
                                    <label class="col-lg-3 col-form-label">BUILDOUT</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <select class="form-select" id="buildout">
                                            <option value=""></option>
                                            <option value="9.5mm Buildout">9.5MM BUILDOUT</option>
                                            <option value="25mm Buildout">25MM BUILDOUT</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divBuildoutPosition">
                                    <label class="col-lg-3 col-form-label required">BUILDOUT POSITION</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <select class="form-select" id="buildoutposition">
                                                <option value=""></option>
                                                <option value="Back of Frame">BACK OF FRAME</option>
                                                <option value="Back of Lip">BACK OF LIP</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divSameSize">
                                    <label class="col-lg-3 col-form-label">SAME SIZE PANELS</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <select class="form-select" id="samesizepanel">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divGapPost">
                                    <label class="col-lg-3 col-form-label">GAP / T-POST</label>
                                    <div class="col-lg-9 col-md-12 col-sm-12">
                                        <div class="row mb-3">
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="divGap1">
                                                <div class="input-group">
                                                    <input type="number" id="gap1" class="form-control" autocomplete="off" placeholder="..... " />
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');"> ? </a>
                                                </div>
                                                <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 1</small>
                                            </div>
                                            
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="divGap2">
                                                <div class="input-group">
                                                    <input type="number" id="gap2" class="form-control" autocomplete="off" placeholder="..... " />
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');"> ? </a>
                                                </div>
                                                <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 2</small>
                                            </div>
                                            
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="divGap3">
                                                <div class="input-group">
                                                    <input type="number" id="gap3" class="form-control" autocomplete="off" placeholder="..... " />
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');"> ? </a>
                                                </div>
                                                <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 3</small>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="divGap4">
                                                <div class="input-group">
                                                    <input type="number" id="gap4" class="form-control" autocomplete="off" placeholder="..... " />
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('T-Post Location');"> ? </a>
                                                </div>
                                                <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 4</small>
                                            </div>
                                            
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="divGap5">
                                                <div class="input-group">
                                                    <input type="number" id="gap5" class="form-control" autocomplete="off" placeholder="..... " />
                                                    <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Gap');"> ? </a>
                                                </div>
                                                <small class="form-hint">* Gap / T Post / Corner Post / Bay Post 5</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-3 mt-6 row" id="divHorizontalTPost">
                                    <label class="col-lg-3 col-form-label">HORIZONTAL T-POST</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="horizontaltpostheight" class="form-control" autocomplete="off" placeholder="..... " />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height</small>
                                    </div>

                                    <div class="col-lg-3 col-md-12 col-sm-12" id="divHorizontalTPostRequired">
                                        <div class="input-group">
                                            <select class="form-select" id="horizontaltpost">
                                                <option value=""></option>
                                                <option value="Yes">YES</option>
                                                <option value="No Post">NO POST</option>
                                            </select>
                                        </div>
                                        <small class="form-hint">* Required</small>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divTiltrodType">
                                    <label class="col-lg-3 col-form-label required">TILTROD TYPE</label>
                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <select class="form-select" id="tiltrodtype">
                                                <option value=""></option>
                                                <option value="Easy Tilt">EASY TILT</option>
                                                <option value="Clearview">CLEARVIEW</option>
                                            </select>
                                            <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo" onclick="return showInfo('Tiltrod Type');"> ? </a>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divTiltrodSplit">
                                    <label class="col-lg-3 col-form-label">SPLIT TILTROD</label>
                                    <div class="col-lg-7 col-md-12 col-sm-12">
                                        <select class="form-select" id="tiltrodsplit"></select>
                                        <small class="form-hint">* Split Tiltrod Rotation</small>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divTiltrodHeight">
                                    <label class="col-lg-3 col-form-label">SPLIT HEIGHT</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="splitheight1" class="form-control" autocomplete="off" placeholder="....." />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 1</small>
                                    </div>

                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="splitheight2" class="form-control" autocomplete="off" placeholder="..... " />
                                            <span class="input-group-text">mm</span>
                                        </div>
                                        <small class="form-hint">* Height 2</small>
                                    </div>
                                </div>

                                <div class="mb-2 mt-4 row" id="divReverseHinged">
                                    <label class="col-lg-3 col-form-label">REVERSE HINGED</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="reversehinged">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-4 row" id="divPelmetFlat">
                                    <label class="col-lg-3 col-form-label">PELMET FLAT PACKED</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="pelmetflat">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-4 row" id="divExtraFascia">
                                    <label class="col-lg-3 col-form-label">EXTRA FASCIA</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="extrafascia">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-4 row" id="divHingesLoose">
                                    <label class="col-lg-3 col-form-label">HINGES LOOSE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="hingesloose">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 mt-5 row" id="divCutOut">
                                    <label class="col-lg-3 col-form-label">FRENCH DOOR CUT-OUT</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="cutout">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divSpecialShape">
                                    <label class="col-lg-3 col-form-label">SPECIAL SHAPE</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="specialshape">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="mb-2 row" id="divTemplateProvided">
                                    <label class="col-lg-3 col-form-label">TEMPLATE PROVIDED</label>
                                    <div class="col-lg-2 col-md-12 col-sm-12">
                                        <select class="form-select" id="templateprovided">
                                            <option value=""></option>
                                            <option value="Yes">YES</option>
                                        </select>
                                    </div>
                                </div>
                                
                                <div class="mb-2 mt-5 row">
                                    <label class="col-lg-3 col-form-label">SPECIAL INFORMATION</label>
                                    <div class="col-lg-8 col-md-12 col-sm-12">
                                        <textarea class="form-control" id="notes" rows="6" placeholder="Your notes ..." style="resize:none;"></textarea>
                                        <span class="form-label-description" id="notescount">0/1000</span>
                                    </div>
                                </div>
                                
                                <div class="mb-2 row">
                                    <label class="col-lg-3 col-form-label">MARK UP</label>
                                    <div class="col-lg-3 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <input type="number" id="markup" class="form-control" autocomplete="off" placeholder="Mark Up ..." />
                                            <span class="input-group-text">%</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <a href="javascript:void(0);" id="submit" class="btn btn-primary">Submit</a>
                            <a href="javascript:void(0);" id="cancel" class="btn btn-danger">Cancel</a>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="markdown">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalSuccess" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-green icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M9 12l2 2l4 -4" /></svg>
                    <h3>Successfully</h3>
                    <div class="text-secondary">Your order has been successfully saved</div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="javascript:void(0);" id="vieworder" class="btn btn-success w-100" data-bs-dismiss="modal">View Order</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalError" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Important Message</h3>
                    <div class="text-secondary" id="errorMsg"></div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn btn-danger w-100" data-bs-dismiss="modal">CLOSE</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalInfo" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalTitle">Modal Title</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="text-secondary">
                        <span id="spanInfo"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn btn-secondary w-100" data-bs-dismiss="modal">OK</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let designIdOri = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8';
        let headerId = '<%= Session("headerId") %>';
        let itemAction = '<%= Session("itemAction") %>';
        let designId = '<%= Session("designId") %>';
        let itemId = '<%= Session("itemId") %>';
        let loginId = '<%= Session("LoginId") %>';
    </script>
    <script src="../Scripts/Order/Panorama.js?v=<%= DateTime.Now.Ticks %>"></script>
</asp:Content>
