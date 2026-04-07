<%@ Page Title="Door" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Door.aspx.vb" Inherits="Order_Door" MaintainScrollPositionOnPostback="true" Debug="true" %>

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
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">door type</label>
                                    <div class="col-lg-4">
                                        <select type="text" name="blindtype" id="blindtype" class="form-select"></select>
                                    </div>  
                                </div>

                                <div class="mb-3 row" id="divTubeType">
                                    <label for="tubetype" class="col-lg-3 text-uppercase fw-bold" id="lblColourType">door product</label>
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

                                    <div class="mb-3 row" >
                                        <label for="room" class="col-lg-3 text-uppercase fw-bold" >room / location</label>
                                        <div class="col-lg-4">
                                            <input type="text" name="room" id="room" class="form-control" autocomplete="off">
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divMounting">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold" >opening</label>
                                        <div class="col-lg-4">
                                            <select name="mounting" id="mounting" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="widthtop" class="col-lg-3 text-uppercase fw-bold">width top x middle</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthtop" id="widthtop" class="form-control " autocomplete="off" placeholder="Width Top...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Top</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthmiddle" id="widthmiddle" class="form-control  " autocomplete="off" placeholder="Width Middle ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Middle</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="widthbottom" class="col-lg-3 text-uppercase fw-bold text-transparent">width bottom x minimum</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthbottom" id="widthbottom" class="form-control  " autocomplete="off" placeholder="Width Bottom ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Bottom</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="widthmin" id="widthmin" class="form-control  " autocomplete="off" placeholder="Width Minimum ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
                                            <small class="form-hint">* Minimum</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="drop" class="col-lg-3 text-uppercase fw-bold text-transparent">height</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <div class="input-group">
                                                <input type="number" min="1" name="drop" id="drop" class="form-control  " autocomplete="off" placeholder="Height ...." />
                                                <span class="input-group-text ">mm</span>
                                            </div>
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

                                    <div class="mb-3 row" id="divLayoutCode">
                                        <label for="layoutcode" class="col-lg-3 text-uppercase fw-bold" >layout</label>
                                        <div class="col-lg-4">
                                            <select name="layoutcode" id="layoutcode" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divHandle">
                                        <label for="handlepostion" class="col-lg-3 text-uppercase fw-bold">handle</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="handlepostion" id="handlepostion" class="form-select"></select>
                                            <small class="form-hint">* Position</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="handlemeasure" id="handlemeasure" class="form-select"></select>
                                            <small class="form-hint">* Measure</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <input type="number" min="1" name="handleheight" id="handleheight" class="form-control  " autocomplete="off"/>
                                            <small class="form-hint">* Height</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divMidrail">
                                        <label for="midrailpostion" class="col-lg-3 text-uppercase fw-bold">midrail</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="midrailpostion" id="midrailpostion" class="form-select"></select>
                                            <small class="form-hint">* Position</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <input type="number" min="1" name="midrailrequest" id="midrailrequest" class="form-control  " autocomplete="off"/>
                                            <small class="form-hint">* Request</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divPetDor">
                                        <label for="petdortype" class="col-lg-3 text-uppercase fw-bold">Pet Dor</label>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="petdortype" id="petdortype" class="form-select"></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-3 col-md-12 col-sm-12">
                                            <select name="petdorposition" id="petdorposition" class="form-select"></select>
                                            <small class="form-hint">* Posotion</small>
                                        </div>
                                    </div>

                                    <div class="mb-3 row" id="divTripleLock">
                                        <label for="triplelock" class="col-lg-3 text-uppercase fw-bold" >Triple Lock</label>
                                        <div class="col-lg-4">
                                            <select name="triplelock" id="triplelock" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divLatchBass">
                                        <label for="latchbass" class="col-lg-3 text-uppercase fw-bold" >Latch Bass</label>
                                        <div class="col-lg-4">
                                            <select name="latchbass" id="latchbass" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBugSeal">
                                        <label for="bugseal" class="col-lg-3 text-uppercase fw-bold" >bugseal aluminium</label>
                                        <div class="col-lg-4">
                                            <select name="bugseal" id="bugseal" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divDoorCloser">
                                        <label for="doorcloser" class="col-lg-3 text-uppercase fw-bold" >door closer</label>
                                        <div class="col-lg-4">
                                            <select name="doorcloser" id="doorcloser" class="form-select"></select>
                                        </div>  
                                    </div>

                                    <div class="mb-3 row" id="divBoldPatio">
                                        <label for="boldpatio" class="col-lg-3 text-uppercase fw-bold" >bold patio lockable</label>
                                        <div class="col-lg-4">
                                            <input type="number" min="1" name="swivelqtyb" id="swivelqtyb" class="form-control  " autocomplete="off"/>
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
        let DESIGNIDORI = "397EB210-E264-49EC-ABF4-DA39B6C018F3";
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
        let URIMETHOD = "/Methods/Order/DoorMethod.aspx";
    </script>

    <script type="text/javascript" src="/Scripts/Order/Door.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

