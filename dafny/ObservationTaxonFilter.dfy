class {:extern} Taxon {
  function {:extern} isSubTaxon(ancestor: Taxon): bool
}

class {:extern} Observation {
  function {:extern} getTaxon(): Taxon
}

function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
  decreases |obs|
{
  if |obs| == 0 then
    []
  else
    var o := obs[0];
    if o.getTaxon().isSubtaxon(root) then
      [o] + FilterBy(root, obs[1..])
    else
      FilterBy(root, obs[1..])
}