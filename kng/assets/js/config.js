"use strict";
var checkUpdateTimer = 10000,
    checkBalanceTimer = 30000,
    maxWaitResultTimer = 120000;
var bet_types = [];

var bet_types = [
        { code: "o1", betID: 1, gameTypeID: 1 },
        { code: "o2", betID: 2, gameTypeID: 1 },
        { code: "o3", betID: 3, gameTypeID: 2 },
        { code: "o4", betID: 4, gameTypeID: 2 },
        { code: "o5", betID: 5, gameTypeID: 3 },
        { code: "o6", betID: 6, gameTypeID: 3 },
        { code: "o7", betID: 7, gameTypeID: 4 },
        { code: "o8", betID: 8, gameTypeID: 4 },
        { code: "o9", betID: 9, gameTypeID: 4 },
        { code: "o10", betID: 10, gameTypeID: 5 },
        { code: "o11", betID: 11, gameTypeID: 5 },
        { code: "o12", betID: 12, gameTypeID: 5 },
        { code: "o13", betID: 13, gameTypeID: 6 },
        { code: "o14", betID: 14, gameTypeID: 6 },
        { code: "o15", betID: 15, gameTypeID: 6 },
        { code: "o16", betID: 16, gameTypeID: 6 },
        { code: "o17", betID: 17, gameTypeID: 7 },
        { code: "o18", betID: 18, gameTypeID: 7 },
        { code: "o19", betID: 19, gameTypeID: 7 },
        { code: "o20", betID: 20, gameTypeID: 7 },
        { code: "o21", betID: 21, gameTypeID: 7 },
        { code: "o22", betID: 22, gameTypeID: 3 }
    ];


function getGameTypeId(props) {
    for (var f = 0; f < bet_types.length; ++f)
        if (bet_types[f].code == props) {
            break;
        }
    return bet_types[f].gameTypeID;
}
function getBetID(props) {
    for (var f = 0; f < bet_types.length; ++f)
        if (bet_types[f].code == props) {
            break;
        }
    return bet_types[f].betID;
}
function getBetTypeCode(props) {
    
    for (var f = 0; f < bet_types.length; ++f)
    {   
        if (bet_types[f].betID == props) {
            break;
        }
    }
    if(bet_types[f] == undefined)
    {
        return false;
    }
    return bet_types[f].code;
}
function initProduct(props) {
    return {
        productID: props.productID,
        title: props.title,
        gID: null,
        gCode: null,
        refID: null,
        drawID: null,
        closeInterval: null,
        previousRes: null,
        previousDrawNo: null,
        serverTime: null,
        resultTime: null,
        countDown: null,
        odds: null,
        result: [],
        stat: null,
        drawResult: null,
        enabled: props.enabled,
        category: props.category,
        index_range: props.index_range,
        url: props.url,
        ready: !0,
        viewreset: null,
        hidden: false,
        fullscreen: false,
        status: "closed",
        previousResult: {
            result: props.previousRes,
            o1: !1,
            o2: !1,
            o3: !1,
            o4: !1,
            o5: !1,
            o6: !1,
            o7: !1,
            o8: !1,
            o9: !1,
            o10: !1,
            o11: !1,
            o12: !1,
            o13: !1,
            o14: !1,
            o15: !1,
            o16: !1,
            o17: !1,
            o18: !1,
            o19: !1,
            o20: !1,
            o21: !1,
            o22: !1,
            sum: null
        }
    };
}
function resetProduct(props) {
    (props.status = "closed"),
        (props.drawResult = null),
        (props.gID = null),
        (props.gCode = null),
        (props.refID = null),
        (props.drawID = null),
        (props.closeInterval = null),
        (props.serverTime = null),
        (props.resultTime = null),
        (props.countDown = null),
        (props.previousRes = null),
        (props.odds = null);
}
for (
    var _iterator = Object.keys(games),
    _isArray = Array.isArray(_iterator),
    _i = 0,
    _iterator = _isArray ? _iterator : _iterator[Symbol.iterator]();
    ;

) {
    var _ref;
    if (_isArray) {
        if (_i >= _iterator.length) break;
        _ref = _iterator[_i++];
    } else {
        if (((_i = _iterator.next()), _i.done)) break;
        _ref = _i.value;
    }
    var key = _ref;
    games[key] = initProduct(games[key]);
}
function shuffleArray(props) {
    var f,
        g,
        e = props.length;
    for (
        props = props.map(function (h) {
            return parseInt(h);
        });
        0 !== e;

    )
        (g = Math.floor(Math.random() * e)),
            (e -= 1),
            (f = props[e]),
            (props[e] = props[g]),
            (props[g] = f);
    return props;
}
function resultArray(props, e) {
    for (var f = [], g = 1; g <= e; g++) f.push(props["n" + g]);
    return f;
}
function numeric(props) {
    return (
        (props += ""),
        (props = props.replace(/,/g, "")),
        (props = parseFloat(props).toFixed(2)),
        (props = parseFloat(props)),
        isNaN(props) ? "" : props
    );
}
function display_numeric(props) {
    var j = 1 < arguments.length && void 0 !== arguments[1] ? arguments[1] : 0,
        l = 2 < arguments.length && void 0 !== arguments[2] && arguments[2];
    if (((props = numeric(props)), "" === props)) return "";
    if (!l && 0 == props) return "";
    for (
        var e = props.toString().split("."), f = /(-?\d+)(\d{3})/, g = e[0];
        f.test(g);

    )
        g = g.replace(f, "$1,$2");
    if (0 < j) {
        for (var h = 1 < e.length ? e[1].substr(0, j) : ""; j > h.length;)
            h += "0";
        return g + "." + h;
    }
    return g;
}
function drawTREND(props) {
    // console.log(props);
    return {

        drawID: parseInt(props.drawID),
        resultTime: props.resultTime,
        sum: props.sum,
        winners: [
            getBetTypeCode(props.bs),
            getBetTypeCode(props.oe),
            getBetTypeCode(props.dt),
            getBetTypeCode(props.hl),
            getBetTypeCode(props.fivee)

        ],
        result: resultArray(props, 20)
    };
}
function chunkArray(props) {
    for (
        var e = props.slice(0).reverse(), f = [], g = [], h = null, j = 0;
        j < e.length;
        j++
    )
        h == e[j].type && 6 > g.length
            ? g.push(e[j])
            : (g.length && (f.push(g), (g = [])), (h = e[j].type), g.push(e[j])),
            j == e.length - 1 && f.push(g);
    var l = f.reverse();
    return (l = l.slice(0, 18)), (l = l.reverse()), l;
}
function trendlotResult(props) {

    var dtchunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.dt),
            trend: drawTREND(m)
        };
    });
    dtchunk = chunkArray(dtchunk);

    var fiveechunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.fivee),
            trend: drawTREND(m)
        };
    });
    fiveechunk = chunkArray(fiveechunk);

    var indexchunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.index),
            trend: drawTREND(m)
        };
    });
    indexchunk = chunkArray(indexchunk);
    
    var bschunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.bs),
            trend: drawTREND(m)
        };
    });
    bschunk = chunkArray(bschunk);

    var oechunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.oe),
            trend: drawTREND(m)
        };
    });
    oechunk = chunkArray(oechunk);

    var hlchunk = props.result.map(function (m) {
        return {
            drawID: parseInt(m.drawID),
            type: getBetTypeCode(m.hl),
            trend: drawTREND(m)
        };
    });
    hlchunk = chunkArray(hlchunk);

    return (
        { dt: dtchunk, index:indexchunk, fivee:fiveechunk, bs: bschunk, oe: oechunk, hl: hlchunk }
    );
}
function getUrlParam(props) {
    for (
        var g,
        e = window.location.href
            .slice(window.location.href.indexOf("?") + 1)
            .split("&"),
        f = 0;
        f < e.length;
        f++
    )
        if (((g = e[f].split("=")), g[0] == props)) return g[1];
    return !1;
}

function showRange(props, e) {
    var f = keno_range[props][e];
    return f.from + "~" + f.to;
}
function checkStakeRecord(props, f) {
    for (
        var g = sessionStorage.getItem("stake-records")
            ? JSON.parse(sessionStorage.getItem("stake-records"))
            : [],
        h = 0,
        j = 0;
        j < g.length;
        j++
    )
        parseInt(g[j].gID) == parseInt(props) &&
            g[j].type == f &&
            (h += parseInt(g[j].stake));
    return h;
}
function checkStakeRecordTotal(props) {
    for (
        var f = sessionStorage.getItem("stake-records")
            ? JSON.parse(sessionStorage.getItem("stake-records"))
            : [],
        g = 0,
        h = f.length - 1;
        0 <= h;
        h--
    )
        parseInt(f[h].gID) == parseInt(props) &&
        (g += parseInt(f[h].stake));
    return g;
}
function clearStakeRecord(props) {
    // console.log("clearStakeRecord:"+props);
    var f = sessionStorage.getItem("stake-records")
        ? JSON.parse(sessionStorage.getItem("stake-records"))
        : [];
    var newf = [];
        for (
            var g = 0,
            h = f.length - 1;
            0 <= h;
            h--
        )
        {
            if(parseInt(f[h].gID) != parseInt(props))
            {
                newf.push(f[h]);
            }

        }
            

        sessionStorage.setItem("stake-records", JSON.stringify(newf));
}
function setStakeRecord(props, f, g) {
    var h = {
        gID: parseInt(props),
        type: f,
        stake: parseInt(g)
    },
        j = sessionStorage.getItem("stake-records")
            ? JSON.parse(sessionStorage.getItem("stake-records"))
            : [];
    j.push(h), sessionStorage.setItem("stake-records", JSON.stringify(j));
}
function setWinner(props, e) {

    var f = e.reduce(function (p, q) {
        return p + q;
    }, 0),
        g = false;

    var hcnt = 0;
    var lcnt = 0;

    for (var i = 0; i <= e.length; i++) {

        if(e[i] != null)
        {

            if(e[i] >= 41 )
            {
                lcnt++;
            }
            else
            {
                hcnt++;
            }

        }
        
    }
    
    var bs = 0;
    if(f>=811)
    {
      bs = 1;
    }
    else if(f<=810)
    {
      bs = 2;
    }

    var index = 0;
    if(f>=811)
    {
      index = 5;
    }
    else if(f<=809)
    {
      index = 6;
    }
    else
    {
        index = 22;
    }

    var oe = 0;
    if(f % 2 == "0")
    {
        oe = 4;
    }
    else
    {
        oe = 3;
    }

    var hl = 0;
    if(hcnt > lcnt)
    {
        hl = 10;
    }
    else if(hcnt < lcnt)
    {
        hl = 12;   
    }
    else
    {
        hl = 11; 
    }

    var dt = 0;
    var dth = Math.floor((f % 100) / 10);
    var dtt = f % 10;

    if(dtt > dth)
    {
        dt = 9;
    }
    else if(dtt < dth)
    {
        dt = 7;   
    }
    else
    {
        dt = 8;   
    }

    var fivee = 0;
    if(f>=210 && f<=695)
    {
      fivee = 17;
    }
    else if(f>=696 && f<=763)
    {
      fivee = 18;
    }
    else if(f>=764 && f<=855)
    {
      fivee = 19;
    }
    else if(f>=856 && f<=923)
    {
      fivee = 20;
    }
    else if(f>=924 && f<=1410)
    {
      fivee = 21;
    }


    (props.previousResult.sum = f),
    (props.previousResult.bs = bs),
    (props.previousResult.oe = oe),
    (props.previousResult.dt = dt),
    (props.previousResult.index = index),
    (props.previousResult.fivee = fivee),

    
    (props.previousResult.hl = hl),
    (props.previousResult.hcnt = hcnt),
    (props.previousResult.lcnt = lcnt),
    (props.previousResult.dcnt = dth),
    (props.previousResult.tcnt = dtt),

    (props.previousResult.o1 = bs == 1),
    (props.previousResult.o2 = bs == 2),
    (props.previousResult.o3 = oe == 3),
    (props.previousResult.o4 = oe == 4),
    (props.previousResult.o5 = index == 5),
    (props.previousResult.o6 = index == 6),

    (props.previousResult.o7 = dt == 7),
    (props.previousResult.o8 = dt == 8),
    (props.previousResult.o9 = dt == 9),

    (props.previousResult.o10 = hl == 10),
    (props.previousResult.o11 = hl == 11),
    (props.previousResult.o12 = hl == 12),

    (props.previousResult.o13 = (bs == 1 && oe == 3)),
    (props.previousResult.o14 = (bs == 1 && oe == 4)),
    (props.previousResult.o15 = (bs == 2 && oe == 3)),
    (props.previousResult.o16 = (bs == 2 && oe == 4)),

    (props.previousResult.o17 = fivee == 17),
    (props.previousResult.o18 = fivee == 18),
    (props.previousResult.o19 = fivee == 19),
    (props.previousResult.o20 = fivee == 20),
    (props.previousResult.o21 = fivee == 21),
    (props.previousResult.o22 = index == 22);
}
function apiCall(props) {
    var e = new Date();
    return (
        (props += -1 === props.indexOf("?") ? "?" : "&"),
        apiUrl + props + "timestamp=" + e.getTime()
    );
}
function slideToLeft(props, e) {
    Velocity(
        props,
        { translateX: [-props.scrollWidth - e, props.offsetWidth + e] },
        {
            duration: 20 * props.scrollWidth,
            easing: "linear",
            complete: slideToLeft.bind(this, props, e)
        }
    );
}