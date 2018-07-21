#if INTERACTIVE
#I __SOURCE_DIRECTORY__
#load "Common.fs"
open Froggy.Common
#else
module Froggy.Properties
  open Froggy.Common
#endif

  type RoundNumber = int
  type Duration<'store> = Permanent | Lasting | Temporary of ('store -> bool)
  type PropertyValueComponent<'t> = Value of 't | Transform of ('t -> 't)
  type PropertyValue<'store, 't> = PropertyValue of (Duration<'store> * PropertyValueComponent<'t>) list
  module PropertyValue =
    let empty = PropertyValue([])
    let computeValue (durationFilter: Duration<_> -> bool) (store: 'store) (PropertyValue(vs): PropertyValue<'store, 't>) =
      let rec applyValue v rest =
        match v with
        | Value v -> v
        | Transform t ->
          let v = eval rest
          t v
      and eval =
        function
          | (duration, _)::rest when durationFilter duration = false ->
            // do NOT apply since it's for a different duration
            eval rest
          | ((Permanent | Lasting), v)::rest ->
            applyValue v rest
          | (Temporary(pred), v)::rest when pred store ->
            applyValue v rest
          | (Temporary(_), _)::rest ->
            // do NOT apply since pred was false
            eval rest
          | v -> matchfail v
      eval vs
    let computePermanentValue store pv = computeValue (function Permanent -> true | _ -> false) store pv
    let computeCurrentValue store pv = computeValue (thunk true) store pv
    let add duration pv (PropertyValue pvs) =
      PropertyValue((duration, pv)::pvs)
