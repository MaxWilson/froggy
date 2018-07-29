#if INTERACTIVE
#I __SOURCE_DIRECTORY__
#load "Common.fs"
#else
module Froggy.Properties
#endif
  open Froggy.Common

  type RoundNumber = int
  type PropertyScope<'store> = Permanent | Lasting | Temporary of ('store -> bool)
  type PropertyValueComponent<'store, 't> = Value of 't | Transform of ('t -> 't)
  type PropertyValue<'store, 't> = PropertyValue of (PropertyScope<'store> * PropertyValueComponent<'store, 't>) list
  module PropertyValue =
    let empty = PropertyValue([])
    let computeValue (scopeFilter: PropertyScope<_> -> bool) (parentLens: Lens<'store, 'store option, 'store option, 'store>) (store: 'store) (PropertyValue(vs): PropertyValue<'store, 't>) =
      let rec applyValue v rest =
        match v with
        | Value v -> v
        | Transform t ->
          let v = eval rest
          t v
      and eval =
        function
          | (scope, _)::rest when scopeFilter scope = false ->
            // do NOT apply since it's for a different scope
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
    let computePermanentValue parentLens store pv = computeValue (function Permanent -> true | _ -> false) parentLens store pv
    let computeCurrentValue parentLens store pv = computeValue (thunk true) parentLens store pv
    let add scope pv (PropertyValue pvs) =
      PropertyValue((scope, pv)::pvs)
