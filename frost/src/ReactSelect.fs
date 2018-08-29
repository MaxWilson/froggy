module Fable.Import.ReactSelect

open Fable.Core
open Fable.Helpers.React
open Fable.Core

  type Option<'t> = Option of 't
  let getValue (Option v) = v
  [<Pojo>]
  type SelectRow<'T> = {
    value: 'T
    label: string
    }
  [<Pojo>]
  type ReactSelectProperties<'Option, 'T> = {
    value: 'T
    options: 'Option array
    onChange: 'Option -> unit
    placeholder: string
    inputValue: string
    getOptionLabel: 'Option -> string
    getOptionValue: 'Option -> string
    controlShouldRenderValue: bool
    }
  let select (props: ReactSelectProperties<_,'T>) = ofImport "default" "react-select" props []
  let selectOfList currentValue inputs onChange =
    select { JsInterop.createEmpty<ReactSelectProperties<_,_>> with getOptionLabel = getValue; getOptionValue = getValue; value = currentValue; options = inputs; onChange = onChange }
