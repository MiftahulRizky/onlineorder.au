<%@ Page Title="Cellora Blind Order" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Cellora.aspx.vb" Inherits="Order_Cellora" MaintainScrollPositionOnPostback="true" Debug="true"  %>


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
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-6">
                                    <div class=" row mb-2">
                                        <label class="col-lg-3">ORDER NUMBER</label>
                                        <div class="col-lg-1 ">:</div>
                                        <div class="col-lg-8 " id="divOrderNo">. . . . .</div>
                                    </div>
                                    <div class=" row">
                                        <label class="col-lg-3">REFERENCE </label>
                                        <div class="col-lg-1 ">:</div>
                                        <div class="col-lg-8 " id="divOrderCust">. . . . .</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- element input -->
            <div class="row">
        
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="cardTitle"></h3>
                        </div>
                        <form action="javascript:void(0)" method="post" id="formSubmit">
                            <div class="card-body">
            
                                <div class="mb-3 row">
                                    <label for="blindtype" class="col-lg-3 text-uppercase fw-bold">type</label>
                                    <div class="col-lg-5">
                                        <select type="text" name="blindtype" id="blindtype" class="form-control"></select>
                                    </div>  
                                </div>
            
                                <div class="mb-3 row" id="divColourType">
                                    <label for="colourtype" class="col-lg-3 text-uppercase fw-bold">colour</label>
                                    <div class="col-lg-5">
                                        <select type="text" name="colourtype" id="colourtype" class="form-control "></select>
                                    </div>  
                                </div>
    
                                <div id="divFormDetail">
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
    
                                    <div class="mb-3 row">
                                        <label for="mounting" class="col-lg-3 text-uppercase fw-bold">Mounting</label>
                                        <div class="col-lg-3">
                                            <select name="mounting" id="mounting" class="form-control ">
                                                <option value=""></option>
                                                <option value="Reveal Fit">REVEAL FIT</option>
                                                <option value="Face Fit">FACE FIT</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="fabrictype" class="col-lg-3 text-uppercase fw-bold">fabric type x colour</label>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabrictype" id="fabrictype" class="form-control "></select>
                                            <small class="form-hint">* Type</small>
                                        </div>
                                        <div class="col-lg-4 col-md-12 col-sm-12">
                                            <select name="fabriccolour" id="fabriccolour" class="form-control "></select>
                                            <small class="form-hint">* Colour</small>
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
    
                                    <div class="mb-3 row">
                                        <label for="controlposition" class="col-lg-3 text-uppercase fw-bold">control side</label>
                                        <div class="col-lg-3">
                                            <select name="controlposition" id="controlposition" class="form-control ">
                                                <option value=""></option>
                                                <option value="RHS">RHS</option>
                                                <option value="LHS">LHS</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="mb-3 row">
                                        <label for="qty" class="col-lg-3 text-uppercase fw-bold">chain length</label>
                                        <div class="col-lg-3">
                                            <div class="input-group">
                                                <input type="number" min="1" name="chainlength" id="chainlength" class="form-control  " autocomplete="off" />
                                                <span class="input-group-text ">mm</span>
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
    
                            <div class="card-footer text-end">
                                <button type="submit" id="btnSubmit" class="btn btn-primary "><i class="ti ti-cloud-up fs-2 me-1"></i>Submit</button>
                                <a href="/order/detail" id="btnCancel"  class="btn btn-danger " >
                                   <i class="ti ti-cancel fs-2 me-1"></i> Cancel
                                </a>
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
        let designIdOri = '35905E47-9B37-485B-A3FD-281BE4E3A94E';
        let headerId = '<%=Session("HeaderId")%>'
        let itemAction = '<%= Session("itemAction") %>';
        let designId = '<%= Session("designId") %>';
        let itemId = '<%= Session("itemId") %>';
        let userId = '<%= Session("userId") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';
        let markupAccess = '<%= Session("MarkUpAccess") %>';
        let uriMethod = "/Methods/Order/CelloraMethod.aspx"
        
    </script>

    <script type="text/javascript" src="/Scripts/Order/Cellora.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

